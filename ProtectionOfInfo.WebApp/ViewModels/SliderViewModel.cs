namespace ProtectionOfInfo.WebApp.ViewModels
{
    public class SliderViewModel
    {
        private static string src = "~/images/";
        private string Name { get; set; } = null!;
        public string Src
        {
            get
            {
                return src + Name;
            }
        }
        public string Description { get; set; } = null!;
        public SliderViewModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
