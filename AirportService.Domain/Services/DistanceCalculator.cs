using AirportService.Domain.Models;

namespace AirportService.Domain.Services
{
    public static class DistanceCalculator
    {
        public static double CalculateDistanceInMiles(Location loc1, Location loc2)
        {
            const double R = 3958.8; // Radius of the Earth in miles
            var lat = ToRadians(loc2.Lat - loc1.Lat);
            var lon = ToRadians(loc2.Lon - loc1.Lon);
            var a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                    Math.Cos(ToRadians(loc1.Lat)) * Math.Cos(ToRadians(loc2.Lat)) *
                    Math.Sin(lon / 2) * Math.Sin(lon / 2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
