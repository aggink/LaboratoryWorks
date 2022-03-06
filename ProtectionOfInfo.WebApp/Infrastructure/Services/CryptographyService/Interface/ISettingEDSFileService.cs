using Microsoft.AspNetCore.Http;
using ProtectionOfInfo.WebApp.Data.CryptographyEntities;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService.Interface
{
    public interface ISettingEDSFileService
    {
        public Task<MyFileInfo?> CreateСertificateAsync(int months);
        public Task<MyFileInfo?> SignFileAsync(IFormFile uploadedFile, IFormFile privateKey, string password);
        public Task<MyFileInfo?> GetOriginalFileAsync(IFormFile uploadedFile);
        public Task<bool> CheckSignFileAsync(IFormFile uploadedFile, IFormFile publicKey, string password);
    }
}
