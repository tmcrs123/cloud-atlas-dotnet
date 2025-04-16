using System.Text.Json.Serialization;

namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class Image
    {
        [JsonPropertyName("imageId")]
        public Guid Id { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }
        
        [JsonPropertyName("legend")]
        public string Legend { get; set; }
    }

    public class CreateImageRequest
    {
        public Guid AtlasId { get; set; }
        public Uri ImageUri { get; set; }
        public string Legend { get; set; }
    }

    public class GetImagesForAtlasRequest
    {
        public Guid AtlasId { get; set; }
    }

    public class UpdateImageRequest
    {
        public Guid AtlasId { get; set; }
        public Guid ImageId { get; set; }
        public string Legend { get; set; }
    }

    public class DeleteImageRequest
    {
        public Guid AtlasId { get; set; }
        public Guid ImageId { get; set; }
    }
}
