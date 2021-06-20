using BL;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Repository : IRepository
    {
        private readonly IServiceScopeFactory scopeFactory;
        private AirportDbContext _airportDbContext;
        public Repository(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            InitCtx();
        }


        private void InitCtx()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
            }
        }


        public List<Plane> GetAirPlanes()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                return _airportDbContext.Plane.ToList();
            }
        }

        public void AddPlane(Plane Plane)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                _airportDbContext.Plane.Add(Plane);
                _airportDbContext.SaveChanges();
            }
        }



        public void AddMovement(Movement movement)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                _airportDbContext.Movement.Add(movement);
                _airportDbContext.SaveChanges();
            }
        }

        public void UpdatePlane(int id, Plane plane)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                if (id == plane.PlaneId)
                {
                    _airportDbContext.Entry(plane).State = EntityState.Modified;
                    _airportDbContext.SaveChanges();
                }
            }
        }

        private bool PlaneExists(int id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                return _airportDbContext.Plane.Any(p => p.PlaneId == id);
            }
        }

        public void RemoveGarbagePlanes()
        {
            List<Plane> allPlanes = GetAirPlanes();
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                foreach (Plane p in allPlanes)
                {
                    if(p.StartedProcess && !p.FinishedProcess)
                    {
                        DeletePlane(p.PlaneId);
                    }
                }
            }
        }

        public void DeletePlane(int planeId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                _airportDbContext = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
                var planeToRemove =  _airportDbContext.Plane.Find(planeId);
                if (planeToRemove != null)
                {
                    _airportDbContext.Plane.Remove(planeToRemove);
                    _airportDbContext.SaveChanges();
                }
            }

        }
    }
}
//// GET: api/Planes
//[HttpGet("GetPlane")]
//public async Task<ActionResult<IEnumerable<Plane>>> GetPlane()
//{
//    return await _context.Plane.ToListAsync();
//}

//// GET: api/Planes/5
//[HttpGet("{id}")]
//public async Task<ActionResult<Plane>> GetPlane(int id)
//{
//    var Plane = await _context.Plane.FindAsync(id);

//    if (Plane == null)
//    {
//        return NotFound();
//    }

//    return Plane;
//}

//// PUT: api/Planes/5
//// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//[HttpPut("{id}")]
//public async Task<IActionResult> PutPlane(int id, Plane Plane)
//{
//    if (id != Plane.PlaneId)
//    {
//        return BadRequest();
//    }

//    _context.Entry(Plane).State = EntityState.Modified;

//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateConcurrencyException)
//    {
//        if (!PlaneExists(id))
//        {
//            return NotFound();
//        }
//        else
//        {
//            throw;
//        }
//    }

//    return NoContent();
//}

//// POST: api/Planes
//// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//[HttpPost]
//public async Task<ActionResult<Plane>> PostPlane(Plane Plane)
//{
//    _context.Plane.Add(Plane);
//    await _context.SaveChangesAsync();

//    return CreatedAtAction("GetPlane", new { id = Plane.PlaneId }, Plane);
//}

//// DELETE: api/Planes/5
//[HttpDelete("{id}")]
//public async Task<IActionResult> DeletePlane(int id)
//{
//    var Plane = await _context.Plane.FindAsync(id);
//    if (Plane == null)
//    {
//        return NotFound();
//    }

//    _context.Plane.Remove(Plane);
//    await _context.SaveChangesAsync();

//    return NoContent();
//}

//private bool PlaneExists(int id)
//{
//    return _context.Plane.Any(e => e.PlaneId == id);
//}

