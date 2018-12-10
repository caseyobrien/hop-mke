﻿using System;
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
            _gtfs.LoadGtfsArchive("sample-feed.zip");
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
        public ActionResult<Stop> GetNearest(string lat, string lng)
        {
            double latitude = double.Parse(lat);
            double longitude = double.Parse(lng);
            Stop stop = GoogleQueries.GetNearestStop(latitude, longitude);
            return stop;
        }

    }
}
