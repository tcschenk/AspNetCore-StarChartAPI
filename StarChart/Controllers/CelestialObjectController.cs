using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
