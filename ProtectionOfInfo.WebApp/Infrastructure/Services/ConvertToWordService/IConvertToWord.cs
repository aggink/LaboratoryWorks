using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToWordService
{
    public interface IConvertToWord
    {
        public Task<MyFileInfo?> CreateDocumentAsync(Check contract);
    }
}
