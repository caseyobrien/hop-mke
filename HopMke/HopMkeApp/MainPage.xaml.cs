using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HopMkeApp
{
    public partial class MainPage : ContentPage
    {
        private HopMkeService _service;

        private double _lat;
        private double _lng;

        public MainPage()
        {
            InitializeComponent();

            /*ILocationTracker locationTracker = DependencyService.Get<ILocationTracker>();
            locationTracker.LocationChanged += OnLocationTrackerLocationChanged;
            locationTracker.StartTracking();*/

            _service = new HopMkeService();
        }

        /*void OnLocationTrackerLocationChanged(object sender, GeographicLocation args)
        {
            _lat = args.Latitude;
            _lng = args.Longitude;
        }*/

        async void OnButtonClicked(object sender, EventArgs e)
        {
            string data = "";
            if (sender == _nearestNBStop)
            {
                data = await _service.NearestNBStop();
            } else if (sender == _nearestSBStop)
            {
                data = await _service.NearestSBStop();
            } else if (sender == _next)
            {
                data = await _service.Next("2");
            }

            _label.Text = data;
        }
    }
}
