namespace ProtectionOfInfo.WebApp.ViewModels.ChatViewModels
{
    public class MessageViewModel
    {
        public string UserName { get; set; } = null!;
        public bool IsMessage { get; set; } = false;
        public string? Message { get; set; }
        public bool IsImage { get; set; } = false;
        public string? Url { get; set; }
        public string? FileNameWithExtension { get; set; }
    }
}
