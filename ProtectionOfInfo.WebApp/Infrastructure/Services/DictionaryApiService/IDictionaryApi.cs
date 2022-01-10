using ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.DictionaryApiService
{
    public interface IDictionaryApi
    {
        Task<OperationResult<Dictionary>> GetWordDefinitionsAsync(string word, string code);
        Task<OperationResult<string>> GetJsonWordDefinitionsAsync(string word, string code);
    }
}
