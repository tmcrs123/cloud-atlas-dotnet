using Cloud_Atlas_Dotnet.Application.Configuration;
using Cloud_Atlas_Dotnet.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Cloud_Atlas_Dotnet_Tests
{
    public class GeocodingServiceTests
    {
        private GeocodingService ArrangeMockService(string jsonResponse)
        {
            // arrange
            var mockLogger = new Mock<ILogger<GeocodingService>>();

            var mockSettings = new Mock<IOptions<AppSettings>>();

            mockSettings.Setup(s => s.Value)
               .Returns(new AppSettings()
               {
                   GeocodingApiKey = "111",
               });

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var url = "https://api.geoapify.com/v1/geocode/search?limit=1&text=neverland&apiKey=111";

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(
                    new HttpResponseMessage()
                    {
                        Content = new StringContent(jsonResponse)
                    }
                );

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);

            var mockHttpFactory = new Mock<IHttpClientFactory>();

            mockHttpFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(mockHttpClient);

            return new GeocodingService(mockHttpFactory.Object, mockSettings.Object, mockLogger.Object);
        }

        [Fact]
        public async void It_ReturnsNull_IfInvalidCoordinates()
        {
            var coordinates = await ArrangeMockService("bananas").GeocodeAtlas(new Guid(), "neverland");

            // Assert
            Assert.Null(coordinates);
        }

        [Fact]
        public async void It_ReturnsNull_IfCoordinatesOutOfBounds()
        {
            var json = "{\"features\":[{\"geometry\":{\"coordinates\":[-91,20]}}]}";

            var coordinates = await ArrangeMockService(json).GeocodeAtlas(new Guid(), "neverland");

            // Assert
            Assert.Null(coordinates);
        }

        [Fact]
        public async void It_ReturnsCoordinates()
        {
            var json = "{\"features\":[{\"geometry\":{\"coordinates\":[42,24]}}]}";

            var coordinates = await ArrangeMockService(json).GeocodeAtlas(new Guid(), "neverland");

            // Assert
            Assert.Equal(42, coordinates.Lat);
            Assert.Equal(24, coordinates.Lng);
        }
    }
}
