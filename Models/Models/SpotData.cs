using Models.Enums;
using System;

namespace Models
{
    public class SpotData
    {
        Spots _spot;
        bool _isAvailable;
        Plane _PlaneOnSpot;
        DateTime? _occupiedSince;
        bool _isPathForwardFine; // Tuple<Spots, bool>: nextSpot(Spots type), isPathFine(bool type)

        public SpotData(Spots spot)
        {
            Spot = spot;
            _isAvailable = true;
            _PlaneOnSpot = null;
            OccupiedSince = null;
            IsPathForwardFine = true;
        }

        public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
        public Plane PlaneOnSpot { get => _PlaneOnSpot; set => _PlaneOnSpot = value; }
        public Spots Spot { get => _spot; set => _spot = value; }
        public DateTime? OccupiedSince { get => _occupiedSince; set => _occupiedSince = value; }
        public bool IsPathForwardFine { get => _isPathForwardFine; set => _isPathForwardFine = value; }
    }
}