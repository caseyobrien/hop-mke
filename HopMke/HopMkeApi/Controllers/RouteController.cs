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
    public class RouteController : Controller
    {
        private readonly Gtfs.Gtfs _gtfs = new Gtfs.Gtfs();

        public RouteController()
        {
            //_gtfs.LoadGtfsArchive("sample-feed.zip");
            _gtfs.LoadGtfsArchive("hop-mke.zip");

        }



        // GET: api/route
        [HttpGet]
        public ActionResult<IEnumerable<Route>> GetRoutes()
        {
            return _gtfs.Routes;
        }

        // GET: api/route/AB
        [HttpGet("{id}")]
        public ActionResult<Route> GetRoute(string id)
        {
            return _gtfs.Routes.FirstOrDefault(r => r.Id == id);
        }
    }
}
