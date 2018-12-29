using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace HopMkeApp
{
    public partial class MainPage : ContentPage
    {
        private HopMkeService _service;

        public MainPage()
        {
            InitializeComponent();

            _service = new HopMkeService();
        }


        async void OnButtonClicked(object sender, EventArgs e)
        {
            double latitude = 0.0;
            double longitude = 0.0; 

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                }
            } 
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            string data = "";
            if (sender == _nearestNBStop)
            {
                data = await _service.NearestStop(latitude, longitude, "NB");
            } else if (sender == _nearestSBStop)
            {
                data = await _service.NearestStop(latitude, longitude, "SB");
            } else if (sender == _next)
            {
                data = await _service.Next("2", "NB");
            }

            _label.Text = data;
        }


        
    }
}
