using System;
using System.Globalization;
using System.IO;
using Google.Maps;
using HopMkeApi;
using HopMkeApi.Google;
using HopMkeApi.Gtfs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HopMkeApiTests
{
    public class StopControllerTests
    {

        public StopControllerTests()
        {
            IConfiguration configuration = TestsConfiguration.GetIConfigurationRoot(Directory.GetCurrentDirectory());
            GoogleSettings googleSettings = configuration.GetSection("Google")
                .Get<GoogleSettings>();
            GoogleSigned.AssignAllServices(new GoogleSigned(googleSettings.ApiKey));
        }

        [Theory]
        [InlineData("36.644464", "-116.406905")]
        public void Nearest_TestLocation_ExpectedResult(string lat, string lng)
        {
            StopController controller = new StopController();

            var result = controller.GetNearest(lat, lng);

            var viewResult = Assert.IsType<OkObjectResult>(result);
            Stop stop = Assert.IsAssignableFrom<Stop>(viewResult.Value);

            Assert.Equal("AMV", stop.Id);
        }
    }
}
