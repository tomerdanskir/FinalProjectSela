using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface ITakeoffsManager
    {
        void Start();
        Plane GetNewPlane(); 
    }
}
