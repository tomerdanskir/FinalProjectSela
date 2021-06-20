using Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGenerator
    {
        public Plane GeneratePlane(Purpose purpose); 
    }
}
