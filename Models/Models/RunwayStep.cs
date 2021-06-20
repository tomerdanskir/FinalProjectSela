using Models.Enums;
using System;

namespace Models
{
    public class RunwayStep
    {
        Spots[] _spots;

        public RunwayStep(Spots[] spots)
        {
            Spots = spots;
        }

        public Spots[] Spots { get => _spots; set => _spots = value; }
    }
}