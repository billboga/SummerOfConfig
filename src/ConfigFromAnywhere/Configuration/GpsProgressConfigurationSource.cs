using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ConfigFromAnywhere.Configuration
{
    public class GpsProgressConfigurationSource : IConfigurationSource
    {
        public GpsProgressConfigurationSource(
            IConfigurationRoot configurationRoot,
            string latitudeStartKey,
            string longitudeStartKey,
            string latitudeEndKey,
            string longitudeEndKey,
            out Action<double, double, string> positionChanged)
        {
            updateSettings(configurationRoot, latitudeStartKey, longitudeStartKey, latitudeEndKey, longitudeEndKey);

            positionChanged = (latitude, longitude, accuracy) =>
            {
                data["gps.distance.progress"] = configurationProvider.GetProgress(latitude, longitude).ToString();
                data["gps.accuracy"] = accuracy;
            };
        }

        private void updateSettings(
            IConfigurationRoot configurationRoot,
            string latitudeStartKey,
            string longitudeStartKey,
            string latitudeEndKey,
            string longitudeEndKey)
        {
            configurationProvider = new GpsProgressConfigurationProvider(
                data,
                double.Parse(configurationRoot[latitudeStartKey]),
                double.Parse(configurationRoot[longitudeStartKey]),
                double.Parse(configurationRoot[latitudeEndKey]),
                double.Parse(configurationRoot[longitudeEndKey]));

            configurationRoot.GetReloadToken().RegisterChangeCallback((configuration) =>
            {
                updateSettings(
                    configuration as IConfigurationRoot,
                    latitudeStartKey,
                    longitudeStartKey,
                    latitudeEndKey,
                    longitudeEndKey);
            }, configurationRoot);
        }

        private IDictionary<string, string> data = new Dictionary<string, string>();
        private GpsProgressConfigurationProvider configurationProvider;

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return configurationProvider;
        }
    }
}
