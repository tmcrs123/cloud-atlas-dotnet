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

    
}
