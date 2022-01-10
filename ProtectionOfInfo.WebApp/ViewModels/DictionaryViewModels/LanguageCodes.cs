using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels
{
    public static class LanguageCodes
    {
        public static string Russian = "ru";
        public static string English = "en";
        public static string French = "fr";
        public static string German = "de";
        public static string Japanese = "ja";
        public static string Chinese = "zh";
        public static IEnumerable<string> Codes
        {
            get
            {
                yield return English;
                yield return Russian;
                yield return French;
                yield return German;
                yield return Japanese;
                yield return Chinese;
            }
        }
        public static Dictionary<string, string> DictionaryCodes()
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("English", English);
            dictionary.Add("Russian", Russian);
            dictionary.Add("French", French);
            dictionary.Add("German", German);
            dictionary.Add("Japanese", Japanese);
            dictionary.Add("Chinese", Chinese);
            return dictionary;
        }
    }
}
