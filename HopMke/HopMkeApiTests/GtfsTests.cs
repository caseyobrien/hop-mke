using System;
using System.Globalization;
using HopMkeApi.Gtfs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HopMkeApiTests
{
    public class GtfsTests
    {
        [Fact]
        public void LoadGtfsArchive_SampleFeed_True()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Assert.True(success);
        }

        [Fact]
        public void LoadGtfsArchive_Null_Exception()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive(null);

            Assert.False(success);
        }

        [Fact]
        public void LoadAgency_SampleFeed_ExpectedFields()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Assert.Equal("DTA", gtfs.Agency.Id);
            Assert.Equal("Demo Transit Authority", gtfs.Agency.Name);
            Assert.Equal("http://google.com", gtfs.Agency.Url);
            Assert.Equal("America/Los_Angeles", gtfs.Agency.TimeZone);
        }

        [Fact]
        public void LoadStops_SampleFeed_ExpectedFieldsFirstStop()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Assert.Equal("FUR_CREEK_RES", gtfs.Stops[0].Id);
            Assert.Equal(36.425288m, gtfs.Stops[0].Latitude);
            Assert.Null(gtfs.Stops[0].ZoneId);
            Assert.Null(gtfs.Stops[0].Url);
        }

        [Fact]
        public void LoadRoutes_SampleFeed_ExpectedFieldsFirstRoute()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Assert.Equal("AB", gtfs.Routes[0].Id);
            Assert.Equal("DTA", gtfs.Routes[0].AgencyId);
            Assert.Equal("10", gtfs.Routes[0].ShortName);
            Assert.Equal("Airport - Bullfrog", gtfs.Routes[0].LongName);
            Assert.Equal(3, gtfs.Routes[0].Type);
            Assert.Null(gtfs.Routes[0].Description);
            Assert.Null(gtfs.Routes[0].Url);
            Assert.Null(gtfs.Routes[0].Color);
            Assert.Null(gtfs.Routes[0].TextColor);
        }

        [Fact]
        public void LoadTrips_SampleFeed_ExpectedFieldsFirstTrip()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Trip first = gtfs.Trips[0];
            Assert.Equal("AB", first.RouteId);
            Assert.Equal("FULLW", first.ServiceId);
            Assert.Equal("AB1", first.Id);
            Assert.Equal("to Bullfrog", first.Headsign);
            Assert.Equal("0", first.DirectionId);
            Assert.Equal("1", first.BlockId);
            Assert.Null(first.ShapeId);
        }

        [Fact]
        public void LoadStopTimes_SampleFeed_ExpectedFieldsFirstStopTime()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            StopTime first = gtfs.StopTimes[0];
            Assert.Equal("STBA", first.TripId);
            TimeSpan time = TimeSpan.ParseExact("06:00:00", "h\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(time, first.ArrivalTime);
            Assert.Equal(time, first.DepartureTime);
            Assert.Equal("STAGECOACH", first.StopId);
            Assert.Equal(1, first.StopSequence);
            Assert.Null(first.StopHeadsign);
            Assert.Null(first.PickupType);
            Assert.Null(first.DropoffTime);
            Assert.Null(first.DistanceTraveled);
        }

        [Fact]
        public void LoadServices_SampleFeed_ExpectedFieldsFirstService()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Service first = gtfs.Services[0];
            Assert.Equal("FULLW", first.Id);
            Assert.True(first.Monday);
            Assert.True(first.Tuesday);
            Assert.True(first.Wednesday);
            Assert.True(first.Thursday);
            Assert.True(first.Friday);
            Assert.True(first.Saturday);
            Assert.True(first.Sunday);
            Assert.Equal(DateTime.ParseExact("20070101", "yyyyMMdd", CultureInfo.InvariantCulture), first.StartDate);
            Assert.Equal(DateTime.ParseExact("20101231", "yyyyMMdd", CultureInfo.InvariantCulture), first.EndDate);
        }

        [Fact]
        public void LoadServiceExceptions_SampleFeed_ExpectedFieldsFirstServiceException()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            ServiceException first = gtfs.ServiceExceptions[0];
            Assert.Equal("FULLW", first.ServiceId);
            Assert.Equal(DateTime.ParseExact("20070604", "yyyyMMdd", CultureInfo.InvariantCulture), first.Date);
            Assert.Equal(2, first.Type);
        }

        [Fact]
        public void SetReferencesForRoute_SampleFeed_AllRoutesReferToAgency()
        {
            Gtfs gtfs = new Gtfs();
            bool success = gtfs.LoadGtfsArchive("sample-feed.zip");

            Assert.All(gtfs.Routes, (route) => route.Agency.Name.Equals("Demo Transit Authority"));
        }
    }
}
