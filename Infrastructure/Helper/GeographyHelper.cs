namespace Infrastructure.Helper
{
    public static class GeographyHelper
    {
        public static decimal? CalculateDistanceKm(decimal lat1, decimal lon1, decimal? lat2, decimal? lon2)
        {
            if (lat2 == null || lon2 == null)
                return null;


            const double R = 6371.0;

            double dLat = DegreesToRadians((double)(lat2 - lat1));
            double dLon = DegreesToRadians((double)(lon2 - lon1));

            double lat1Rad = DegreesToRadians((double)lat1);
            double lat2Rad = DegreesToRadians((double)lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c;

            return (decimal)distance; // Trả lại dạng decimal
        }

        private static double DegreesToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
