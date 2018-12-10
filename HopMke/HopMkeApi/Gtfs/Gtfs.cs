using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HopMkeApi.Gtfs
{
    public class Gtfs
    {
        // TO-DO: Make a singleton class.
        public Agency Agency { get; set; }
        public Stop[] Stops { get; set; }
        public Route[] Routes { get; set; }
        public Trip[] Trips { get; set; }
        public StopTime[] StopTimes { get; set; }
        public Service[] Services { get; set; }
        public ServiceException[] ServiceExceptions { get; set; }

        public Gtfs()
        {

        }

        public bool LoadGtfsArchive(string gtfsArchiveFileName)
        {
            string extractionDirectory = Path.GetFileNameWithoutExtension(gtfsArchiveFileName);

            ZipFile.ExtractToDirectory(gtfsArchiveFileName, extractionDirectory, true);

            string[] gtfsFiles = Directory.GetFiles(extractionDirectory);
            Agency = (Agency) LoadObjectsFromFile("Agency", extractionDirectory + "/agency.txt")[0];
            Stops = (Stop[]) LoadObjectsFromFile("Stop", extractionDirectory + "/stops.txt");
            Routes = (Route[]) LoadObjectsFromFile("Route", extractionDirectory + "/routes.txt");
            Trips = (Trip[]) LoadObjectsFromFile("Trip", extractionDirectory + "/trips.txt");
            StopTimes = (StopTime[]) LoadObjectsFromFile("StopTime", extractionDirectory + "/stop_times.txt");
            Services = (Service[]) LoadObjectsFromFile("Service", extractionDirectory + "/calendar.txt");

            if (File.Exists(extractionDirectory + "/calendar_dates.txt")) 
                ServiceExceptions = (ServiceException[]) LoadObjectsFromFile("ServiceException", extractionDirectory + "/calendar_dates.txt");

            SetReferences(extractionDirectory);

            return true;
        }

        private void SetReferences(string extractionDirectory)
        {
            SetReferencesForRoutes();
            SetReferencesForTrips();
            SetReferencesForStopTimes();
            if (File.Exists(extractionDirectory + "/calendar_dates.txt"))
                SetReferncesForServiceExceptions();
        }

        private void SetReferencesForRoutes()
        {
            foreach (Route route in Routes)
            {
                route.Agency = Agency;
            }
        }

        private void SetReferencesForTrips()
        {
            foreach (Trip trip in Trips)
            {
                trip.Route = Routes.FirstOrDefault(r => r.Id == trip.RouteId);
                trip.Service = Services.FirstOrDefault(s => s.Id == trip.ServiceId);
                // trip.Shape = Services.FirstOrDefault(s => s.Id == trip.ShapeId);
            }
        }

        private void SetReferencesForStopTimes()
        {
            foreach (StopTime stopTime in StopTimes)
            {
                stopTime.Trip = Trips.FirstOrDefault(t => t.Id == stopTime.TripId);
                stopTime.Stop = Stops.FirstOrDefault(s => s.Id == stopTime.StopId);
            }
        }

        private void SetReferncesForServiceExceptions()
        {
            foreach (ServiceException serviceException in ServiceExceptions)
            {
                serviceException.Service = Services.FirstOrDefault(s => s.Id == serviceException.ServiceId);
            }
        }

        private GtfsObject[] LoadObjectsFromFile(string type, string gtfsFile)
        {
            JArray jsonArray = CsvToJson(gtfsFile);
            switch (type)
            {
                case "Agency":
                    return jsonArray.ToObject<Agency[]>();
                case "Stop":
                    return jsonArray.ToObject<Stop[]>();
                case "Route":
                    return jsonArray.ToObject<Route[]>();
                case "Trip":
                    return jsonArray.ToObject<Trip[]>();
                case "StopTime":
                    return jsonArray.ToObject<StopTime[]>();
                case "Service":
                    return jsonArray.ToObject<Service[]>();
                case "ServiceException":
                    return jsonArray.ToObject<ServiceException[]>();
            }
            return null;
        }

        private JArray CsvToJson(string csvFile)
        {
            string[] lines = File.ReadAllLines(csvFile);
            string[] fields = lines[0].Split(',');

            JArray jsonArray = new JArray();

            for(int i=1; i < lines.Length; i++)
            {
                JObject jsonObject = new JObject();
                string[] values = lines[i].Split(',');
                for (int j=0; j < values.Length; j++)
                {
                    jsonObject[fields[j]] = values[j].Equals("") ? null : values[j];
                }
                jsonArray.Add(jsonObject);
            }

            return jsonArray;
        }
    }

    public abstract class GtfsObject { }


    public class Agency : GtfsObject
    {
        [Key]
        [JsonProperty("agency_id")]
        public string Id { get; set; }
        [JsonProperty("agency_name")]
        public string Name { get; set; }
        [JsonProperty("agency_url")]
        public string Url { get; set; }
        [JsonProperty("agency_timezone")]
        public string TimeZone { get; set; }
    }

    public class Stop : GtfsObject
    {
        [Key]
        [JsonProperty("stop_id")]
        public string Id { get; set; }
        [JsonProperty("stop_name")]
        public string Name { get; set; }
        [JsonProperty("stop_lat")]
        public decimal Latitude { get; set; }
        [JsonProperty("stop_lon")]
        public decimal Longitude { get; set; }
        [JsonProperty("zone_id")]
        public string ZoneId { get; set; }
        [JsonProperty("stop_url")]
        public string Url { get; set; }
    }

    public class Route : GtfsObject
    {
        [Key]
        [JsonProperty("route_id")]
        public string Id { get; set; }
        [JsonProperty("agency_id")]
        public string AgencyId { get; set; }
        public Agency Agency { get; set; }
        [JsonProperty("route_short_name")]
        public string ShortName { get; set; }
        [JsonProperty("route_long_name")]
        public string LongName { get; set; }
        [JsonProperty("route_desc")]
        public string Description { get; set; }
        [JsonProperty("route_type")]
        public int Type { get; set; }
        [JsonProperty("route_url")]
        public string Url { get; set; }
        [JsonProperty("route_color")]
        public string Color { get; set; }
        [JsonProperty("route_text_color")]
        public string TextColor { get; set; }

    }

    public class StopTime : GtfsObject
    {
        [Key]
        public int Id { get; set; }
        [JsonProperty("trip_id")]
        public string TripId { get; set; }
        public Trip Trip { get; set; }
        [JsonProperty("arrival_time")]
        public TimeSpan ArrivalTime{ get; set; }
        [JsonProperty("departure_time")]
        public TimeSpan DepartureTime { get; set; }
        [JsonProperty("stop_id")]
        public string StopId { get; set; }
        public Stop Stop { get; set; }
        [JsonProperty("stop_sequence")]
        public int StopSequence { get; set; }
        [JsonProperty("stop_headsign")]
        public string StopHeadsign { get; set; }
        [JsonProperty("pickup_type")]
        public int? PickupType { get; set; }
        [JsonProperty("dropoff_time")]
        public TimeSpan? DropoffTime { get; set; }
        [JsonProperty("shape_dist_traveled")]
        public decimal? DistanceTraveled { get; set; }

    }

    public class Trip : GtfsObject
    {
        [Key]
        [JsonProperty("trip_id")]
        public string Id { get; set; }
        [JsonProperty("route_id")]
        public string RouteId { get; set; }
        public Route Route { get; set; }
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        public Service Service { get; set; }
        [JsonProperty("trip_headsign")]
        public string Headsign { get; set; }
        [JsonProperty("trip_short_name")]
        public string ShortName { get; set; }
        [JsonProperty("shape_id")]
        public string ShapeId { get; set; }
        public Shape Shape { get; set; }
        [JsonProperty("trip_bikes_allowed")]
        public bool BikesAllowed { get; set; }
        [JsonProperty("direction_id")]
        public string DirectionId { get; set; }
        [JsonProperty("block_id")]
        public string BlockId { get; set; }
    }


    public class Service : GtfsObject
    {
        [Key]
        [JsonProperty("service_id")]
        public string Id { get; set; }
        [JsonProperty("monday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Monday { get; set; }
        [JsonProperty("tuesday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Tuesday { get; set; }
        [JsonProperty("wednesday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Wednesday { get; set; }
        [JsonProperty("thursday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Thursday { get; set; }
        [JsonProperty("friday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Friday { get; set; }
        [JsonProperty("saturday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Saturday { get; set; }
        [JsonProperty("sunday")]
        [JsonConverter(typeof(BooleanJsonConverter))]
        public bool Sunday { get; set; }
        [JsonProperty("start_date")]
        [JsonConverter(typeof(DateJsonConverter))]
        public DateTime StartDate { get; set; }
        [JsonProperty("end_date")]
        [JsonConverter(typeof(DateJsonConverter))]
        public DateTime EndDate { get; set; }
    }

    public class ServiceException : GtfsObject
    {
        [Key]
        public string Id { get; set; }
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        public Service Service { get; set; }
        [JsonProperty("date")]
        [JsonConverter(typeof(DateJsonConverter))]
        public DateTime Date { get; set; }
        [JsonProperty("exception_type")]
        public int Type { get; set; }
    }

    public class Shape : GtfsObject
    {
        [Key]
        public string Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int PointSequence { get; set; }
        public decimal DistanceTraveled { get; set; }
    }
}
