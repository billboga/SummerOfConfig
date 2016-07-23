using Microsoft.Extensions.Configuration;
using System;

namespace ConfigFromAnywhere.Configuration
{
    public static class GpsProgressConfigurationExtensions
    {
        public static IConfigurationBuilder AddGpsProgress(
            this IConfigurationBuilder configurationBuilder,
            IConfigurationRoot configurationRoot,
            string latitudeStartKey,
            string longitudeStartKey,
            string latitudeEndKey,
            string longitudeEndKey,
            out Action<double, double, string> positionChanged)
        {
            return configurationBuilder.Add(
                new GpsProgressConfigurationSource(
                    configurationRoot,
                    latitudeStartKey,
                    longitudeStartKey,
                    latitudeEndKey,
                    longitudeEndKey,
                    out positionChanged));
        }
    }
}
