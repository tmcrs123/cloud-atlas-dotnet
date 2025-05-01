namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public Coordinates(double lat, double lng)
        {
            if (lat < -90d || lat > 90d) throw new InvalidOperationException("Latitude out of bounds");
            if (lng < -180d || lng > 180d) throw new InvalidOperationException("Longitude out of bounds");

            Lat = lat;
            Lng = lng;
        }
    }
}
