using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities
{
    public class Meaning
    {
        [JsonPropertyName("partOfSpeech")]
        public string? PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")]
        public ICollection<Description>? Definitions { get; set; }
    }
}
