using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels
{
    public class DictionaryViewModel
    {
        public string? Word { get; set; }
        public string? Origin { get; set; }
        public ICollection<MeaningViewModel>? Meanings { get; set; }
    }
}
