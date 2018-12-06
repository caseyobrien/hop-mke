using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HopMkeApi.Models.Gtfs
{
    public class Gtfs
    {
        public Agency Agency { get; set; }
        public Stop[] Stops { get; set; }
        public Route[] Routes { get; set; }
        public Trip[] Trips { get; set; }
        public StopTime[] StopTimes { get; set; }
        public Service[] Services { get; set; }
        public ServiceException[] ServiceExceptions { get; set; }

        private readonly ILogger _logger;

        public Gtfs(ILogger<Gtfs> logger)
        {
            _logger = logger;
        }

        public bool LoadGtfsArchive(string gtfsArchiveFileName)
        {
            string extractionDirectory = Path.GetFileNameWithoutExtension(gtfsArchiveFileName);

            try
            { 
                ZipFile.ExtractToDirectory(gtfsArchiveFileName, extractionDirectory, true);
            } 
            catch (InvalidDataException)
            {
                _logger.LogError("File {gtfsArchiveFileName} is not a valid zip archive", gtfsArchiveFileName);
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogError("File {gtfsArchiveFileName} not found.", gtfsArchiveFileName);
                return false;
            }
            catch (Exception)
            {
                _logger.LogError("Error with file {gtfsArchiveFileName}", gtfsArchiveFileName);
                return false;
            }

            string[] gtfsFiles = Directory.GetFiles(extractionDirectory);

            Agency = LoadAgency(extractionDirectory + "/agency.txt");
            Stops = LoadStops(extractionDirectory + "/stops.txt");
            Routes = LoadRoutes(extractionDirectory + "/routes.txt");
            Trips = LoadTrips(extractionDirectory + "/trips.txt");
            StopTimes = LoadStopTimes(extractionDirectory + "/stop_times.txt");
            Services = LoadServices(extractionDirectory + "/calendar.txt");
            ServiceExceptions = LoadServiceExceptions(extractionDirectory + "/calendar_dates.txt");

            return true;
        }

        private Agency LoadAgency(string agencyGtfsFile)
        {
            JArray agenciesJson = CsvToJson(agencyGtfsFile);
            JToken agencyJson = agenciesJson[0];
            Agency agency = agencyJson.ToObject<Agency>();

            return agency;
        }

        private Stop[] LoadStops(string stopsGtfsFile)
        {
            JArray stopsJson = CsvToJson(stopsGtfsFile);
            Stop[] stops = stopsJson.ToObject<Stop[]>();
            return stops;
        }

        private Route[] LoadRoutes(string routesGtfsFile)
        {
            JArray routesJson = CsvToJson(routesGtfsFile);
            Route[] routes = routesJson.ToObject<Route[]>();
            return routes;
        }

        private Trip[] LoadTrips(string tripsGtfsFile)
        {
            JArray tripsJson = CsvToJson(tripsGtfsFile);
            Trip[] trips = tripsJson.ToObject<Trip[]>();
            return trips;
        }

        private StopTime[] LoadStopTimes(string stopTimesGtfsFile)
        {
            JArray stopTimesJson = CsvToJson(stopTimesGtfsFile);
            StopTime[] stopTimes = stopTimesJson.ToObject<StopTime[]>();
            return stopTimes;
        }

        private Service[] LoadServices(string calendarGtfsFile)
        {
            JArray servicesJson = CsvToJson(calendarGtfsFile);
            Service[] services = JsonConvert.DeserializeObject<Service[]>(servicesJson.ToString(), new BooleanJsonConverter(), new DateJsonConverter());//servicesJson.ToObject<Service[]>();
            return services;
        }

        private ServiceException[] LoadServiceExceptions(string calendarDatesGtfsFile)
        {
            JArray serviceExceptionsJson = CsvToJson(calendarDatesGtfsFile);
            ServiceException[] serviceExceptions = JsonConvert.DeserializeObject<ServiceException[]>(serviceExceptionsJson.ToString(), new DateJsonConverter());
            return serviceExceptions;
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

    

    public class Agency
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

    public class Stop
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

    public class Route
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

    public class StopTime
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

    public class Trip
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


    public class Service
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

    public class ServiceException
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

    public class Shape
    {
        [Key]
        public string Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int PointSequence { get; set; }
        public decimal DistanceTraveled { get; set; }
    }
}
