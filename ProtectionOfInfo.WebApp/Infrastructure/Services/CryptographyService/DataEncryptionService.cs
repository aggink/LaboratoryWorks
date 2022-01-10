using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    public class DataEncryptionService : IDataEncryptionService
    {
        private readonly string hash;
        public DataEncryptionService() 
        {
            // задаем const пароль для расшифровки и шифровки БД
            // и хешируем его
            MD5 md5 = MD5.Create();
            var Key = md5.ComputeHash(Encoding.UTF8.GetBytes("qwerty"));
            hash = Convert.ToBase64String(Key);
        }

        public bool CheckPassword(string hashPassword)
        {
            // проверка на совпадение const хеш пароля и хеш пароля пользователя
            if(hash == hashPassword)
            {
                return true;
            }

            return false;
        }

        public string HashPassword(string password)
        {
            // хешируем пользовательский пароль
            MD5 md5 = MD5.Create();

            var Key = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            var HashPassword = Convert.ToBase64String(Key);

            md5.Dispose();
            return HashPassword;
        }

        public string Encrypt_Aes(string plainText)
        {
            var Key = Convert.FromBase64String(hash);
            //Выполняет симметричное шифрование и дешифрование с помощью реализации CAPI алгоритма симметричного шифрования AES.
            AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider()
            {
                // KeySize = ... размер ключа 64, 128, 256 бит
                // BlockSize = ... размер IV 64, 128 (aes не поддерживает 256)
                // IV =  ... в ECB он не участвует, в других CipherMode участвует
                Key = Key, // хеш ключ для шиврования и расшиврования
                Mode = CipherMode.ECB // режим шифрования
            };

            byte[] encrypted;

            //Создайте шифровальщик для выполнения преобразования потока.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            //Создайте потоки, используемые для шифрования.
            //Создает поток, резервным хранилищем которого является память.
            using (var msEncrypt = new MemoryStream())
            {
                //Определяет поток, который связывает потоки данных с криптографическими преобразованиями.
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    //Реализует TextWriter для записи символов в поток в определенной кодировке.
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Записываем все данные в поток.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            aesAlg.Dispose();

            //возвращаем зашифрованные байты из потока
            return Convert.ToBase64String(encrypted);
        }

        public string? Decrypt_Aes(string _cipherText, string hashpassword)
        {
            var Key = Convert.FromBase64String(hashpassword);
            //Выполняет симметричное шифрование и дешифрование с помощью реализации CAPI алгоритма симметричного шифрования AES.
            AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider()
            {
                Key = Key,
                Mode = CipherMode.ECB
            };

            byte[] cipherText = Convert.FromBase64String(_cipherText);

            // Создаем дешифратор для выполнения преобразования потока.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            string? plaintext = null;
            try
            {
                // Создаем потоки, используемые для дешифрования.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Считываем дешифрованные байты из потока дешифрования и помещаем их в строку.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch 
            {
                plaintext = null;
            }

            aesAlg.Dispose();

            return plaintext;
        }
    }
}
