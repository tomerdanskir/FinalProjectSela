using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IRepository
    {
        public List<Plane> GetAirPlanes();
        public void AddPlane(Plane plane);

        public void AddMovement(Movement movement);

        public void UpdatePlane(int id, Plane plane);
        public void RemoveGarbagePlanes();

    }
}
