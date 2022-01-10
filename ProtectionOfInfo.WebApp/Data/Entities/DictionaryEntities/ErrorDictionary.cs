using System.Text.Json.Serialization;

namespace ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities
{
    public class ErrorDictionary
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("resolution")]
        public string? Resolution { get; set; }
    }
}
