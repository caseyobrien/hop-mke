using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Maps;
using HopMkeApi.Google;
using HopMkeApi.Gtfs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HopMkeApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class StopController : Controller
    {
        private readonly Gtfs.Gtfs _gtfs = new Gtfs.Gtfs();

        public StopController()
        {
            //_gtfs.LoadGtfsArchive("sample-feed.zip");
            _gtfs.LoadGtfsArchive("hop-mke.zip");
        }



        // GET: api/stop
        [HttpGet]
        public ActionResult<IEnumerable<Stop>> GetRoutes()
        {
            return _gtfs.Stops;
        }

        // GET: api/stop/AB
        [HttpGet("{id}")]
        public ActionResult<Stop> GetStop(string id)
        {
            return _gtfs.Stops.FirstOrDefault(s => s.Id == id);
        }

        [HttpGet("nearest")]
        [ProducesResponseType(200, Type = typeof(Stop))]
        [ProducesResponseType(404)]
        public IActionResult GetNearest(string lat, string lng, string dir)
        {
            double latitude = double.Parse(lat);
            double longitude = double.Parse(lng);
            Stop stop = GoogleQueries.GetNearestStop(latitude, longitude, dir);
            return stop != null ? Ok(stop) : (IActionResult)NotFound();
        }

        [HttpGet("next/{id}")]
        [ProducesResponseType(200, Type = typeof(Stop))]
        [ProducesResponseType(404)]
        public IActionResult GetNext(string id)
        {
            StopTime stopTime = _gtfs.StopTimes.Where(st => st.ArrivalTime >= DateTime.Now.TimeOfDay).OrderBy(st => st.ArrivalTime).FirstOrDefault(st => st.StopId == id);
            return stopTime != null ? Ok(stopTime) : (IActionResult)NotFound();
        }


    }
}
