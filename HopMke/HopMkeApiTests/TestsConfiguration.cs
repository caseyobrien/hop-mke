using System;
using Microsoft.Extensions.Configuration;

namespace HopMkeApiTests
{
    public static class TestsConfiguration
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddUserSecrets("443a1efc-70ae-4516-a69a-774d74ded0b1")
                .Build();
        }
    }
}
