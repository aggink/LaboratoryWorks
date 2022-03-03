using System.Security.Cryptography;
using System.Text;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    /// <summary>
    ///  // aggink: update summary - 28.02.2022 23:18:07
    /// </summary>
    public interface IAlgorithmsCryptography
    {
        public byte[] Encrypt(byte[] data, string password);
        public byte[] Decrypt(byte[] data, string password);
    }
}
