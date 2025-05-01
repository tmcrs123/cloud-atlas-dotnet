namespace Cloud_Atlas_Dotnet.Application.Services
{
    public class GeocodingService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GeocodingService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void GeocodeAtlas(Guid atlasId, string placeIdentifier)
        {
            var client = _httpClientFactory.CreateClient();

            client.
        }
    }
}
