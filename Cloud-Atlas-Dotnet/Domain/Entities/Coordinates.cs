namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public Coordinates(double lat, double lng)
        {
            if(!CoordinatesWithinBoundaries(lat, lng))
            {
                throw new InvalidOperationException($"Tried to create coordinates with invalid boundaries: Lat: {lat}; Lng: {lng}");
            }

            Lat = lat;
            Lng = lng;
        }

        public static bool CoordinatesWithinBoundaries(double lat, double lng)
        {
            if (lat < -90d || lat > 90d || lng < -180d || lng > 180d) return false;
            return true;
        }
    }
}
