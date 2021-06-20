using Models.Enums;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BL
{
    public class PlanesGenerator : IGenerator
    {
        public Plane GeneratePlane(Purpose purpose)
        {
            Plane newPlane;
            var values = Enum.GetValues(typeof(Countries));
            var randomCountry = (Countries)values.GetValue(new Random().Next(values.Length));
            if (purpose.Equals(Purpose.Landing))
                newPlane = new Plane(randomCountry, Countries.ISR, Guid.NewGuid().GetHashCode(), Purpose.Landing);
            else
                newPlane = new Plane(Countries.ISR, randomCountry, Guid.NewGuid().GetHashCode(), Purpose.Takeoff);

            Trace.WriteLine($"***CREATION: {newPlane.ToString()}");
            return newPlane;
        }
    }
}
