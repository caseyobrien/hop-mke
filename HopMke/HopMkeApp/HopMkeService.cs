using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HopMkeApp
{
    public class HopMkeService
    {
        private readonly HttpClient _client;

        private readonly string _stopNextEndpoint = "https://hop-mke-api.herokuapp.com/api/stop/next/";
        private readonly string _stopNearestEndpoint = "https://hop-mke-api.herokuapp.com/api/stop/nearest/";

        public HopMkeService()
        {
            _client = new HttpClient();
            _client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<string> NearestNBStop()
        {
            var uri = new Uri(string.Format(_stopNearestEndpoint, string.Empty));

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return null;
        }

        public async Task<string> NearestSBStop()
        {
            var uri = new Uri(string.Format(_stopNearestEndpoint, string.Empty));

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return null;
        }

        public async Task<string> Next(string stopId)
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
