namespace ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities
{
    public class MyFileInfo
    {
        public MyFileInfo(byte[] file, string fileName, string extension)
        {
            File = file;
            Extension = extension;
            FileName = fileName;
        }

        public byte[] File { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
    }
}
