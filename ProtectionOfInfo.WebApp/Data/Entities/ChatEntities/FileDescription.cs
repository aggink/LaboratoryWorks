namespace ProtectionOfInfo.WebApp.Data.Entities.ChatEntities
{
    public class FileDescription
    {
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public byte[] Data { get; set; } = null!;
    }
}
