using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using HopMkeApi.Gtfs;
using static Google.Maps.DistanceMatrix.DistanceMatrixResponse;

namespace HopMkeApi.Google
{
    public static class GoogleQueries
    {
        private static readonly Gtfs.Gtfs _gtfs = new Gtfs.Gtfs();

        static GoogleQueries()
        {
            //_gtfs.LoadGtfsArchive("sample-feed.zip");
            _gtfs.LoadGtfsArchive("hop-mke.zip");
        }

        public static Stop GetNearestStop(double latitude, double longitude, string direction)
        {
            var distanceMatrixRequest = new DistanceMatrixRequest();
            distanceMatrixRequest.Units = Units.imperial;
            distanceMatrixRequest.Mode = TravelMode.walking;
            distanceMatrixRequest.AddOrigin(new LatLng(latitude, longitude));

            IEnumerable<Stop> filteredStops = null;
            if (direction != null)
            {
                switch(direction.ToUpper())
                {
                    case "NORTHBOUND":
                    case "NB":
                        var trips = _gtfs.Trips.Where(t => t.Route.ShortName == "MNBWD").Distinct();
                        filteredStops = _gtfs.StopTimes.Join(trips, st => st.TripId, t => t.Id, (st, t) => st.Stop).Distinct();
                        direction = "NB";
                        break;
                    case "SOUTHBOUND":
                    case "SB":
                        trips = _gtfs.Trips.Where(t => t.Route.ShortName == "MSBWD").Distinct();
                        filteredStops = _gtfs.StopTimes.Join(trips, st => st.TripId, t => t.Id, (st, t) => st.Stop).Distinct();
                        direction = "SB";
                        break;
                }
            }
            else
            {
                filteredStops = _gtfs.Stops;
            }


            foreach (Stop stop in filteredStops)
            {
                distanceMatrixRequest.AddDestination(
                    new LatLng(stop.Latitude, stop.Longitude));
            }


            var response = new DistanceMatrixService().GetResponse(distanceMatrixRequest);
            if (response.Status == ServiceResponseStatus.Ok && response.Rows.Length > 0)
            {
                DistanceMatrixElement[] distances = response.Rows[0].Elements.OrderBy(e => e, new DistanceMatrixElementComparer()).ToArray();
                DistanceMatrixElement closest = distances[0];
                int stopIndex = Array.IndexOf(response.Rows[0].Elements, closest);
                return filteredStops.ToArray()[stopIndex];
            }
            return null;
        }
    }

    public class DistanceMatrixElementComparer : IComparer<DistanceMatrixElement>
    {
        public int Compare(DistanceMatrixElement x, DistanceMatrixElement y)
        {
            long xDistance = long.Parse(x.distance.Value.ToString());
            long yDistance = long.Parse(y.distance.Value.ToString());
            return xDistance.CompareTo(yDistance);
        }
    }
}
