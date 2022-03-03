namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface
{
    /// <summary>
    ///  // aggink: update summary - 01.03.2022 17:10:24
    /// </summary>
    public interface ICryptographyService
    {
        public byte[] Encrypt(byte[] data, string algorithm, string password);
        public byte[] Decrypt(byte[] data, string password);
    }
}
