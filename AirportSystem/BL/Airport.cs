using Interfaces;
using Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Models.Enums;
using System.Linq;
using Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BL
{
    public class Airport : IAirport
    {
        ILandingsManager _landingsManager;
        ITakeoffsManager _takeoffsManager;
        IRepository _repository;

        Queue<Plane> _landingsWaiting;
        Queue<Plane> _takeoffsWaiting;
        int landingsEnded = 0, takeoffsEnded = 0;
        Runway _runway;
        List<Plane> _planes;

        private bool _started = false;
        private List<Plane> _dbPlanes;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

        public Airport(ILandingsManager landingsManager, ITakeoffsManager takeoffsManager, IRepository repository, IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _hubContext = hubContext;
            _repository = repository;
            _landingsManager = landingsManager;
            _takeoffsManager = takeoffsManager;
            _landingsWaiting = new Queue<Plane>();
            _takeoffsWaiting = new Queue<Plane>();
            _runway = new Runway();
            _planes = new List<Plane>();
        }

        public void KeepAirportAlive()
        {
            if (!StopReceivingPlanes(_landingsWaiting))
                ReceiveLandings();
            if (!StopReceivingPlanes(_takeoffsWaiting))
                ReceiveTakeoffs();


            Dictionary<Spots, List<Plane>> destinations = new Dictionary<Spots, List<Plane>>();
            // building dict for all spots potentionals with Planes that can occupy them next step
            foreach (Plane p in _planes)
            {
                Spots[] nextSpots;
                nextSpots = _runway.GetNextSpot(p);
                foreach (var spot in nextSpots)
                {
                    if (_runway.CanMoveTowards(spot))
                    {
                        if (destinations.ContainsKey(spot))
                            destinations[spot].Add(p);
                        else
                            destinations.Add(spot, new List<Plane>() { p });
                    }
                }
            }

            //TODO: maybe value should be Plane.id -> in case it exists by same referrence on all instances
            // also maybe "cleaner"
            Dictionary<Spots, Plane> movements = new Dictionary<Spots, Plane>();

            // building dict with spot and Plane to move into, using special algo in case of collision
            foreach (var spot in destinations)
            {
                List<Plane> availablePlanes = spot.Value.FindAll(p => !movements.ContainsValue(p));
                if (availablePlanes != null && availablePlanes.Count != 0)
                {
                    var PlaneToMove = availablePlanes.Count == 1 ? availablePlanes[0] : PlaneToMoveAlgo(availablePlanes);
                    movements.Add(spot.Key, PlaneToMove);
                }
            }

            // todo: dict need to be sent as json on signalR in order to update the ui
            string json = JsonConvert.SerializeObject(movements);
            _hubContext.Clients.All.BroadcastMessage(json);

            // performing movements
            foreach (var move in movements)
            {
                Movement newMove = new Movement(move.Value, move.Value.CurrentSpot, move.Key);
                Spots srcSpot = default;
                // if Plane is on runway now
                if (move.Value.CurrentSpot.HasValue)
                {
                    srcSpot = move.Value.CurrentSpot.Value;
                    _runway.ClearSpot(move.Value.CurrentSpot.Value);
                }

                newMove.MovementDateTime = UpdatePlane(move.Value, move.Key);
                if (!move.Key.Equals(Spots.OUT))
                    _runway.UpdateSpot(move.Key, move.Value);

                _repository.AddMovement(newMove);

                string srcSpotName = srcSpot == default ? "Start Point Outside" : srcSpot.ToString();
                Trace.WriteLine(move.Value.ToString() + $" Moved from {srcSpotName} to {move.Key.ToString()} at {move.Value.SpotArrivalDateTime}");
            }

            Trace.WriteLine($"\nTakeOffs finished: {takeoffsEnded}\nLandings finished: {landingsEnded}\n");

        }

        // updates plane ds and db. 
        // returns the exact moment of plane's update
        private DateTime UpdatePlane(Plane plane, Spots dstSpot)
        {
            DateTime movementTime;
            if (dstSpot.Equals(Spots.OUT))
            {
                if (plane.PlanesPurpose.Equals(Purpose.Landing)) landingsEnded++;
                else takeoffsEnded++;
                plane.FinishedProcess = true;
                plane.FinishedOnDateTime = DateTime.Now;
                movementTime = plane.FinishedOnDateTime.Value;
                _repository.UpdatePlane(plane.PlaneId, plane);
                RemovePlane(plane.SerialID);
            }
            else
            {
                var currPlaneFromList = _planes.Find(p => p.SerialID.Equals(plane.SerialID));
                currPlaneFromList.CurrentSpot = dstSpot;
                currPlaneFromList.SpotArrivalDateTime = DateTime.Now;
                movementTime = currPlaneFromList.SpotArrivalDateTime;
            }
            //if planes come from outside
            if (!plane.StartedProcess)
            {
                var currPlaneFromList = _planes.Find(p => p.SerialID.Equals(plane.SerialID));
                currPlaneFromList.StartedProcess = true;
                _repository.UpdatePlane(currPlaneFromList.PlaneId, currPlaneFromList);
                DequeueWaitingPlanesList(plane.PlanesPurpose);
            }

            return movementTime;
        }

        private void DequeueWaitingPlanesList(Purpose purpose)
        {
            if (purpose.Equals(Purpose.Landing)) _landingsWaiting.Dequeue();
            else _takeoffsWaiting.Dequeue();
        }

        private void RemovePlane(int id)
        {
            _planes.Remove(_planes.Find(p => p.SerialID.Equals(id)));
        }

        /*
         Algo Preference:
        1. more havy spots right behind him
        2. earlier to stand on his spot
         (waiting queue Planes dont count )
        -----------------
        Moreover resonable Options:
        3. is it last spot before finishing 
        4. the one on the longer runway

         */
        private Plane PlaneToMoveAlgo(List<Plane> Planes)
        {
            List<Plane> bestPlaneByMaxLoad = MaxLoadMaker(Planes);
            if (bestPlaneByMaxLoad.Count == 1)
            {
                return bestPlaneByMaxLoad[0];
            }
            var best = bestPlaneByMaxLoad.OrderBy(p => p.SpotArrivalDateTime).ToList<Plane>();
            // if longest waiter is a queue waiting Plane
            if (!best.FirstOrDefault().StartedProcess)
            {
                var longestWaiterOnRunway = best.FirstOrDefault(p => p.StartedProcess);
                if (longestWaiterOnRunway != null) return longestWaiterOnRunway;
            }
            return best.FirstOrDefault();
        }

        private List<Plane> GetPlanesAreAboutToFinish(List<Plane> Planes)
        {
            return Planes.FindAll(p =>
            {
                return (p.CurrentSpot.HasValue) && _runway.IsSpotIsOneBeforeLast(p.PlanesPurpose, p.CurrentSpot.Value);
            }
            );
        }

        // returns the Planes that on their previous step there are the most "stucked" Planes
        private List<Plane> MaxLoadMaker(List<Plane> Planes)
        {
            List<Plane> maxHarmful = new List<Plane>();
            int maximumWeight = 0, currWeight = 0;
            for (int i = 0; i < Planes.Count; i++)
            {
                currWeight = _runway.GetAmountOfPlanesStuckBehind(Planes[i]);
                if (currWeight > maximumWeight)
                {
                    maxHarmful = new List<Plane>() { Planes[i] };
                    maximumWeight = currWeight;
                }
                else if (currWeight == maximumWeight)
                    maxHarmful.Add(Planes[i]);
            }
            return maxHarmful;
        }

        // starts the simulator 
        public void Start()
        {
            if (_dbPlanes == null)
                _dbPlanes = (_repository.GetAirPlanes())?.OrderBy(p => p.ReceivedOnSystemDateTime).ToList().FindAll(p => !p.StartedProcess);
            KeepAirportAlive();
        }

        public void ReceiveLandings()
        {
            Plane newPlane;
            if (_dbPlanes != null && _dbPlanes.Count != 0)
            {
                newPlane = _dbPlanes.FirstOrDefault(p => p.PlanesPurpose.Equals(Purpose.Landing));
                if (newPlane != null)
                    _dbPlanes.Remove(newPlane);
                else
                {
                    newPlane = _landingsManager.GetNewPlane();
                    _repository.AddPlane(newPlane);
                }
            }
            else
            {
                newPlane = _landingsManager.GetNewPlane();
                _repository.AddPlane(newPlane);
            }
 
            _landingsWaiting.Enqueue(newPlane);
            _planes.Add(newPlane);
        }

        public void ReceiveTakeoffs()
        {
            Plane newPlane;
            if (_dbPlanes != null && _dbPlanes.Count != 0)
            {
                newPlane = _dbPlanes.FirstOrDefault(p => p.PlanesPurpose.Equals(Purpose.Takeoff));
                if (newPlane != null)
                    _dbPlanes.Remove(newPlane);
                else
                {
                    newPlane = _takeoffsManager.GetNewPlane();
                    _repository.AddPlane(newPlane);
                }
            }
            else
            {
                newPlane = _takeoffsManager.GetNewPlane();
                _repository.AddPlane(newPlane);
            }

            _takeoffsWaiting.Enqueue(newPlane);
            _planes.Add(newPlane);
        }

        public bool StopReceivingPlanes(Queue<Plane> waiting)
        {
            return waiting?.Count() == 10;
        }
    }

}
