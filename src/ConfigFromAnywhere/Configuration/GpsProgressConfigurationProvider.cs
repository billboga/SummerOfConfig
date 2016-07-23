using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ConfigFromAnywhere.Configuration
{
    public class GpsProgressConfigurationProvider : ConfigurationProvider
    {
        public GpsProgressConfigurationProvider(
            IDictionary<string, string> data,
            double latitudeStart,
            double longitudeStart,
            double latitudeEnd,
            double longitudeEnd)
        {
            Data = data;
            this.latitudeStart = latitudeStart;
            this.latitudeEnd = latitudeEnd;
            this.longitudeStart = longitudeStart;
            this.longitudeEnd = longitudeEnd;

            absoluteDistance = getAngularDistanceBetweenPoints(latitudeStart, longitudeStart, latitudeEnd, longitudeEnd);
        }

        private readonly double absoluteDistance;
        private readonly double latitudeEnd;
        private readonly double latitudeStart;
        private readonly double longitudeEnd;
        private readonly double longitudeStart;

        /// <summary>
        /// Gets percent-progress as a decimal.
        /// </summary>
        public double GetProgress(double latitude, double longitude)
        {
            var distanceFromStart = getAngularDistanceBetweenPoints(latitudeStart, longitudeStart, latitude, longitude);
            var distanceFromEnd = getAngularDistanceBetweenPoints(latitudeEnd, longitudeEnd, latitude, longitude);

            /**
             * We are taking the average to make sure progress is relative to both
             * start and end-points. If we focus on just the end-point, then the "start"
             * becomes anywhere along the circumference of the imaginary circle where the center is
             * the end-point and the radius is the `absoluteDistance`.
             */
            var averageDistance = (distanceFromStart + (absoluteDistance - distanceFromEnd)) / 2;

            return averageDistance / absoluteDistance;
        }

        /// <summary>
        /// Gets the distance between two coords.
        /// Note: this uses the haversine formula instead of spherical law of cosines
        /// since the former does better over smaller distances.
        /// Also note: the larger the difference in latitude, the higher degree of inaccuracy.
        /// This is due to the formula's assumption of a spherical Earth.
        /// Ref. https://en.wikipedia.org/wiki/Haversine_formula
        /// Ref. http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <returns>Non-unit of measure. To get unit-distance, multiply by Earth's mean radius (i.e. mi or km).</returns>
        private double getAngularDistanceBetweenPoints(
            double latitudeStart,
            double longitudeStart,
            double latitudeEnd,
            double longitudeEnd)
        {
            var latitudeDifference = toRadians(latitudeEnd - latitudeStart);
            var longitudeDifference = toRadians(longitudeEnd - longitudeStart);

            var haversine =
                Math.Pow(Math.Sin(latitudeDifference / 2), 2) +
                Math.Cos(toRadians(latitudeStart)) * Math.Cos(toRadians(latitudeEnd)) *
                Math.Pow(Math.Sin(longitudeDifference / 2), 2);

            var angularDistance = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));

            return angularDistance;
        }

        private double toRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
