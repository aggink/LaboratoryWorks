using System.Security.Cryptography;
using System.Text;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    /// <summary>
    ///  // aggink: update summary - 28.02.2022 23:36:46
    /// </summary>
    public abstract class CryptographyBase
    {
        protected const string AESName = "aes";
        protected const string DESName = "des";

        protected const int SizeMD5 = 16;

        protected byte[] CreateHashMD5(string password)
        {
            byte[] hash;

            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return hash;
        }
    }
}
