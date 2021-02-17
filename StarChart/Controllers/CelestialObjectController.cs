using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestial)
        {
            _context.CelestialObjects.Add(celestial);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestial.Id }, celestial);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestial)
        {
            var oldCelestial = _context.CelestialObjects.FirstOrDefault(elem => elem.Id == id);
            if (oldCelestial != null)
            {
                oldCelestial.Name = celestial.Name;
                oldCelestial.OrbitalPeriod = celestial.OrbitalPeriod;
                oldCelestial.OrbitedObjectId = celestial.OrbitedObjectId;
                _context.CelestialObjects.Update(oldCelestial);
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.FirstOrDefault(elem => elem.Id == id);
            if (celestial != null)
            {
                celestial.Name = name;
                _context.CelestialObjects.Update(celestial);
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestials = new List<CelestialObject>();
            foreach (var celestial in _context.CelestialObjects)
            {
                if (celestial.Id == id || celestial.OrbitedObjectId == id)
                {
                    celestials.Add(celestial);
                }
            }
            if (celestials.Count > 0)
            {
                _context.CelestialObjects.RemoveRange(celestials);
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestial = _context.CelestialObjects.FirstOrDefault(elem => elem.Id == id);
            if (celestial != null)
            {
                celestial.Satellites = getSatellitesById(id);
                return Ok(celestial);
            }
            return NotFound();
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestials = new List<CelestialObject>();
            foreach (var celestial in _context.CelestialObjects)
            {
                if (celestial.Name == name)
                {
                    celestial.Satellites = getSatellitesById(celestial.Id);
                    celestials.Add(celestial);
                }
            }
            if (celestials.Count > 0)
            {
                return Ok(celestials);
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            foreach (var celestial in _context.CelestialObjects)
            {
                celestial.Satellites = getSatellitesById(celestial.Id);
            }
            return Ok(_context.CelestialObjects);
        }

        private List<CelestialObject> getSatellitesById(int id)
        {
            var orbitals = new List<CelestialObject>();
            foreach (var orbited in _context.CelestialObjects)
            {
                if (orbited.Id == id)
                {
                    orbitals.Add(orbited);
                }
            }
            return orbitals;
        }
    }
}
