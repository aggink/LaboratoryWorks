using System;
using System.Security.Cryptography;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    /// <summary>
    ///  // aggink: update summary - 28.02.2022 21:18:18
    /// </summary>
    public class AesCryptography : CryptographyBase, IAlgorithmsCryptography
    {
        public byte[] Decrypt(byte[] data, string password)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("password");

            var Key = CreateHashMD5(password);

            byte[] decrypted;
            using (Aes aesAlg = Aes.Create())
            {
                var size = aesAlg.BlockSize / 8;

                byte[] IV = new byte[size];
                byte[] encrypted = new byte[data.Length - IV.Length - SizeMD5];

                Array.Copy(data, SizeMD5, IV, 0, IV.Length);
                Array.Copy(data, IV.Length + SizeMD5, encrypted, 0, encrypted.Length);

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                decrypted = aesAlg.DecryptCbc(encrypted, IV, PaddingMode.PKCS7);
            }

            return decrypted;
        }

        public byte[] Encrypt(byte[] data, string password)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("password");

            var Key = CreateHashMD5(password);
            byte[] IV;

            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                encrypted = aesAlg.EncryptCbc(data, aesAlg.IV, PaddingMode.PKCS7);
            }

            var alg = CreateHashMD5(AESName);
            byte[] result = new byte[encrypted.Length + IV.Length + alg.Length];
            Array.Copy(alg, 0, result, 0, alg.Length);
            Array.Copy(IV, 0, result, alg.Length, IV.Length);
            Array.Copy(encrypted, 0, result, alg.Length + IV.Length, encrypted.Length);

            return result;
        }
    }
}
