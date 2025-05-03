using Cloud_Atlas_Dotnet.Application.Configuration;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Cloud_Atlas_Dotnet.Application.Services
{
    public interface IGeocodingService
    {
        Task<Coordinates> GeocodeAtlas(Guid atlasId, string placeIdentifier);
    }

    public class GeocodingService : IGeocodingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<AppSettings> _settings;
        private readonly ILogger<GeocodingService> _logger;

        public GeocodingService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> settings, ILogger<GeocodingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings;
            _logger = logger;
        }

        public async Task<Coordinates> GeocodeAtlas(Guid atlasId, string placeIdentifier)
        {
            var client = _httpClientFactory.CreateClient("GeocodingClient");

            StringBuilder sb = new StringBuilder("https://api.geoapify.com/v1/geocode/search?limit=1");

            sb.Append($"&text={placeIdentifier}");

            var apiKey = _settings.Value.GeocodingApiKey;
            if (apiKey is null) throw new InvalidOperationException("API key not provided for geocoding service");

            sb.Append($"&apiKey={apiKey}");

            var msg = await client.GetAsync(sb.ToString());

            var json = await msg.Content.ReadAsStringAsync();

            JsonDocument jdoc;
            JsonElement jsonCoordinates;

            try
            {
                jdoc = JsonDocument.Parse(json);
                jsonCoordinates = jdoc.RootElement
                .GetProperty("features")[0]
                .GetProperty("geometry")
                .GetProperty("coordinates");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse JSON coordinates from Geocoding API");
                return null;
            }

            var lat = jsonCoordinates[0].GetDouble();
            var lng = jsonCoordinates[1].GetDouble();

            if (double.IsNaN(lat) || double.IsNaN(lng))
            {
                return null;
            }

            if (!Coordinates.CoordinatesWithinBoundaries(lat, lng))
            {
                return null;
            }
            ;

            return new Coordinates(lat, lng);
        }
    }
}
