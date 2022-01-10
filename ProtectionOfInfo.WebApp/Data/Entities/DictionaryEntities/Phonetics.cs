using System.Text.Json.Serialization;

namespace ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities
{
    public class Phonetics
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("audio")]
        public string? Audio { get; set; }
    }
}
