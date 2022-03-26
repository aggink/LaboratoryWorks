namespace ProtectionOfInfo.WebApp.ViewModels.ChatViewModels
{
    public class FileUrlViewModel
    {
        public bool IsImage { get; set; } = false;
        public string UserName { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Extension { get; set; } = null!;
    }
}
