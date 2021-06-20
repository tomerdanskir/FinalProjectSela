using Interfaces;
using Models;
using System.Diagnostics;
using Models.Enums;
using System.Threading.Tasks;

namespace BL
{
    public class TakeoffsManager: ITakeoffsManager, ICreate
    {
        bool _started = false;
        IGenerator _generator;

        public TakeoffsManager(IGenerator generator)
        {
            _generator = generator;
        }

        public  Plane CreatePlane()
        {
            return  _generator.GeneratePlane(Purpose.Takeoff);
        }

        public  Plane GetNewPlane()
        {
            return  CreatePlane();
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
