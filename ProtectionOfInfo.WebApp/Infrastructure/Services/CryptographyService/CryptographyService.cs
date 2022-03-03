using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface;
using System;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    /// <summary>
    ///  // aggink: update summary - 01.03.2022 17:10:19
    /// </summary>
    public class CryptographyService : CryptographyBase, ICryptographyService
    {

        public byte[] Decrypt(byte[] data, string password)
        {
            byte[] alg = new byte[SizeMD5];
            Array.Copy(data, 0, alg, 0, SizeMD5);
            var nameAlg = Convert.ToBase64String(alg);

            string AES = Convert.ToBase64String(CreateHashMD5(AESName));
            string DES = (string)Convert.ToBase64String(CreateHashMD5(DESName));

            IAlgorithmsCryptography _cryptographyService = new AesCryptography();
            if (nameAlg == DES) _cryptographyService = new DesCryptography();

            try
            {
                return _cryptographyService.Decrypt(data, password);
            }
            catch
            {
                return new byte[0];
            }
        }

        public byte[] Encrypt(byte[] data, string algorithm, string password)
        {
            IAlgorithmsCryptography _cryptographyService;
            switch (algorithm.ToLower())
            {
                case "aes":
                    _cryptographyService = new AesCryptography();
                    break;
                case "des":
                    _cryptographyService = new DesCryptography();
                    break;
                default:
                    _cryptographyService = new AesCryptography();
                    break;
            }
            try
            {
                return _cryptographyService.Encrypt(data, password);
            }
            catch
            {
                return new byte[0];
            }
        }

    }
}
