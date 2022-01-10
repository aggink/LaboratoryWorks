using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService
{
    public interface IDataEncryptionService
    {
        bool CheckPassword(string hashPassword);
        string HashPassword(string password);
        string Encrypt_Aes(string plainText);
        string? Decrypt_Aes(string _cipherText, string hashpassword);
    }
}
