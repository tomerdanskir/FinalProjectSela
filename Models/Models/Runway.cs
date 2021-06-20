using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Runway
    {
        Dictionary<Spots, SpotData> _spots;
        LinkedList<RunwayStep> _takeOffRunway;
        LinkedList<RunwayStep> _landingRunway;

        public Dictionary<Spots, SpotData> Spots { get => _spots; set => _spots = value; }
        public LinkedList<RunwayStep> TakeOffRunway { get => _takeOffRunway; set => _takeOffRunway = value; }
        public LinkedList<RunwayStep> LandingRunway { get => _landingRunway; set => _landingRunway = value; }

        public Runway()
        {
            Spots = new Dictionary<Spots, SpotData>();
            var allSpots = Enum.GetValues(typeof(Spots));
            foreach (var s in allSpots)
            {
                Spots.Add(((Spots)s), new SpotData((Spots)s));
            }

            TakeOffRunway = new LinkedList<RunwayStep>();
            TakeOffRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Six, Enums.Spots.Seven }));
            TakeOffRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Eight }));
            TakeOffRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Four }));

            LandingRunway = new LinkedList<RunwayStep>();
            LandingRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.One }));
            LandingRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Two }));
            LandingRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Three }));
            LandingRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Four }));
            LandingRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Five }));
            LandingRunway.AddLast(new RunwayStep(new Spots[] { Enums.Spots.Six, Enums.Spots.Seven }));
        }

        public Spots[] GetNextSpot(Plane p)
        {
            Spots[] nextSpots = new Spots[1];
            LinkedList<RunwayStep> matchingList = ReturnRelevantList(p.PlanesPurpose);
          
            if (!p.StartedProcess)
            {
                return matchingList.First.Value.Spots;
            }

            var tmpNode = matchingList.First;
            LinkedListNode<RunwayStep> dstSpotsNode = null;
            while (tmpNode != null)
            {
                if (tmpNode.Value.Spots.Any(s => s.Equals(p.CurrentSpot)))
                {
                    dstSpotsNode = tmpNode.Next; ;
                    break;
                }
                tmpNode = tmpNode.Next;
            }

            if (dstSpotsNode == null)
            {
                nextSpots[0] = Enums.Spots.OUT;
                return nextSpots;
            }
            return dstSpotsNode.Value.Spots;
        }

        public bool CanMoveTowards(Spots spot)
        {
            return Spots[spot].IsAvailable && Spots[spot].IsPathForwardFine;
        }

        public void ClearSpot(Spots value)
        {
            Spots[value].IsAvailable = true;
            Spots[value].OccupiedSince = null;
            Spots[value].PlaneOnSpot = null;
        }

        public void UpdateSpot(Spots value, Plane Plane)
        {
            Spots[value].IsAvailable = false;
            Spots[value].OccupiedSince = DateTime.Now;
            Spots[value].PlaneOnSpot = Plane;
        }

        //gets how many Planes stuck behind Planes spot
        //considering only Planes at the same direction, amount is 0 for waiting Planes and first spot Planes
        public int GetAmountOfPlanesStuckBehind(Plane Plane)
        {
            if (!Plane.CurrentSpot.HasValue) return 0;
            int counterAmount = 0;
            LinkedList<RunwayStep> matchingList = ReturnRelevantList(Plane.PlanesPurpose);
            
            // if Plane on first spot of runway
            if (matchingList.First.Value.Spots.Any(s => s.Equals(Plane.CurrentSpot.Value))) return 0;

            var tmpNode = matchingList.First;
            while (tmpNode != null)
            {
                // if next node is Planes spot
                if (tmpNode.Next != null && tmpNode.Next.Value.Spots.Any(s => s.Equals(Plane.CurrentSpot.Value)))
                    break;
                tmpNode = tmpNode.Next;
            }

            //not possible for null..
            if (tmpNode != null)
            {
                foreach (var spot in tmpNode.Value.Spots)
                {
                    if (Spots[spot].PlaneOnSpot != null && Spots[spot].PlaneOnSpot.PlanesPurpose.Equals(Plane.PlanesPurpose))
                        counterAmount++;
                }
            }

            return counterAmount;
        }

        // according to inputed purpose
        private LinkedList<RunwayStep> ReturnRelevantList(Purpose p)
        {
            if (p.Equals(Purpose.Landing)) return _landingRunway;
            return _takeOffRunway;
        }

        
        public bool IsSpotIsOneBeforeLast(Purpose PlanesPurpose, Spots value)
        {
            LinkedList<RunwayStep> matchingList = ReturnRelevantList(PlanesPurpose);
            var tmp = matchingList.First;
            while (tmp != null)
            {
                if (tmp.Value.Spots.Any(s => s.Equals(value)))
                    return tmp.Next.Next == null;
                tmp = tmp.Next;
            }
            return false;
        }
    }
}
