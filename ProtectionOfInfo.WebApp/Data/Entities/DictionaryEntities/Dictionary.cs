using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities
{
    public class Dictionary
    {
        [JsonPropertyName("word")]
        public string? Word { get; set; }

        [JsonPropertyName("phonetic")]
        public string? Phonetic { get; set; }

        [JsonPropertyName("origin")]
        public string? Origin { get; set; }

        [JsonPropertyName("phonetics")]
        public ICollection<Phonetics>? Phonetics { get; set; }

        [JsonPropertyName("meanings")]
        public ICollection<Meaning>? Meanings { get; set; }
    }
}
