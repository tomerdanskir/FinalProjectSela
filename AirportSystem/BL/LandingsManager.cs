using Interfaces;
using Models;
using Models.Enums;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BL
{
    public class LandingsManager : ILandingsManager, ICreate

    {
        bool _started = false;
        IGenerator _generator;

        public LandingsManager(IGenerator generator)
        {
            _generator = generator;
        }

        public Plane CreatePlane()
        {
            return _generator.GeneratePlane(Purpose.Landing);
        }

        public Plane GetNewPlane()
        {
            return CreatePlane();
        }
        public void Start()
        {
            if (_started)
            {
                Trace.WriteLine($"==>{this.GetType().Name} already started");
                return;
            }

            _started = true;
            Trace.WriteLine($"==>{this.GetType().Name} is starting");
        }
    }
}
