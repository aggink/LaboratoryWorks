using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels
{
    public class DescriptionViewModel
    {
        public string? Definition { get; set; }
        public string? Example { get; set; }
        public ICollection<string>? Synonyms { get; set; }
        public ICollection<string>? Antonyms { get; set; } 
    }
}
