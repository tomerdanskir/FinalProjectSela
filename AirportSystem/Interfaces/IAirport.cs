using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IAirport
    {
        void Start();
        void ReceiveLandings();
        void ReceiveTakeoffs();

        bool StopReceivingPlanes(Queue<Plane> waiting);
    }
}
