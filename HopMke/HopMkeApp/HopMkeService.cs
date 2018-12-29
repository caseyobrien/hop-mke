using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace HopMkeApp
{
    public class HopMkeService
    {
        private readonly HttpClient _client;

        private readonly string _stopNextEndpoint = "https://hop-mke-api.herokuapp.com/api/stop/next/";
        private readonly string _stopNearestEndpoint = "https://hop-mke-api.herokuapp.com/api/stop/nearest";

        public HopMkeService()
        {
            _client = new HttpClient();
            _client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<string> NearestStop(double latitude, double longitude, string direction)
        {
            var uriBuilder = new UriBuilder(_stopNearestEndpoint);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["lat"] = latitude.ToString();
            query["lng"] = longitude.ToString();
            query["dir"] = direction;
            uriBuilder.Query = query.ToString();
            var uri = uriBuilder.Uri;

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return null;
        }

        public async Task<string> Next(string stopId, string direction)
        {
            var uri = new Uri(string.Format(_stopNextEndpoint + stopId, string.Empty));

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return null;
        }



    }
}
