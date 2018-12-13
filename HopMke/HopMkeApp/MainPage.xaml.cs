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

        public MainPage()
        {
            InitializeComponent();

            _service = new HopMkeService();
        }

        async void OnNextButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Cicked.");
            string data = await _service.RefreshDataAsync();
            _label.Text = data;
        }
    }
}
