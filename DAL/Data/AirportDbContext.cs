using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;


namespace DAL.Data
{
    public class AirportDbContext : DbContext
    {
        public AirportDbContext (DbContextOptions<AirportDbContext> options)
            : base(options)
        {
        }

        public DbSet<Plane> Plane { get; set; }
        public DbSet<Movement> Movement { get; set; }

       
    }
}
