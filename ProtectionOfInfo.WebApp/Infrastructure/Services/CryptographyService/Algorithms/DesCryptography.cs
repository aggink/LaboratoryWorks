using System;
using System.Security.Cryptography;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    /// <summary>
    ///  // aggink: update summary - 01.03.2022 17:10:03
    /// </summary>
    public class DesCryptography : CryptographyBase, IAlgorithmsCryptography
    {
        public byte[] Decrypt(byte[] data, string password)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("password");


            byte[] decrypted;
            using (DES desAlg = DES.Create())
            {
                int size = desAlg.BlockSize / 8;

                byte[] Key = new byte[size];
                byte[] IV = new byte[size];
                byte[] encrypted = new byte[data.Length - IV.Length - SizeMD5];

                Array.Copy(CreateHashMD5(password), 0, Key, 0, size);
                Array.Copy(data, SizeMD5, IV, 0, IV.Length);
                Array.Copy(data, IV.Length + SizeMD5, encrypted, 0, encrypted.Length);

                desAlg.Key = Key;
                desAlg.IV = IV;

                decrypted = desAlg.DecryptCbc(encrypted, IV, PaddingMode.PKCS7);
            }

            return decrypted;
        }

        public byte[] Encrypt(byte[] data, string password)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("password");

            byte[] IV;
            byte[] Key = new byte[8];

            byte[] encrypted;
            using (DES desAlg = DES.Create())
            {
                int size = desAlg.BlockSize / 8;

                Array.Copy(CreateHashMD5(password), 0, Key, 0, 8);
                desAlg.Key = Key;

                desAlg.GenerateIV();
                IV = desAlg.IV;

                encrypted = desAlg.EncryptCbc(data, desAlg.IV, PaddingMode.PKCS7);
            }

            var alg = CreateHashMD5(DESName);
            byte[] result = new byte[encrypted.Length + IV.Length + alg.Length];
            Array.Copy(alg, 0, result, 0, alg.Length);
            Array.Copy(IV, 0, result, alg.Length, IV.Length);
            Array.Copy(encrypted, 0, result, alg.Length + IV.Length, encrypted.Length);

            return result;
        }
    }
}
