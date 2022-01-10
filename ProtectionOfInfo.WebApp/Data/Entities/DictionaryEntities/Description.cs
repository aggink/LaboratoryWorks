using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities
{
    public class Description
    {
        [JsonPropertyName("definition")]
        public string? Definition { get; set; }

        [JsonPropertyName("example")]
        public string? Example { get; set; }
        [JsonPropertyName("synonyms")]
        public ICollection<string>? Synonyms { get; set; }
        [JsonPropertyName("antonyms")]
        public ICollection<string>? Antonyms { get; set; }
    }
}
