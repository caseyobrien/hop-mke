using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HopMkeApi.Gtfs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HopMkeApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : Controller
    {
        private readonly Gtfs.Gtfs _gtfs = new Gtfs.Gtfs();

        public TripController()
        {
            _gtfs.LoadGtfsArchive("sample-feed.zip");
        }



        // GET: api/trip
        [HttpGet]
        public ActionResult<IEnumerable<Trip>> GetRoutes()
        {
            return _gtfs.Trips;
        }

        // GET: api/trip/AB
        [HttpGet("{id}")]
        public ActionResult<Trip> GetTrip(string id)
        {
            return _gtfs.Trips.FirstOrDefault(s => s.Id == id);
        }
    }
}
