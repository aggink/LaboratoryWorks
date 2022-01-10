using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager
{
    public interface IBookManager : IMyManager<Book>
    {
        Task<OperationResult<TEntity>> GetEmptyEntitiesWithParamAsync<TEntity>(TEntity entity) where TEntity : IBookParamViewModel;
        Task<OperationResult<TEntity>> СheckRelationshipsWithOtherEntitiesAsync<TEntity>(TEntity entity) where TEntity : IBookParamViewModel;
        Task<OperationResult<List<Book>>> SearchWithOtherEntitiesAsync(string ObjectSearch, string Search);
        Task<OperationResult<List<Book>>> SortWithOtherEntitiesAsync(string ObjectSort, string TypeSort);
        Task<OperationResult<decimal>> MaxBookPrice();
        Task<OperationResult<decimal>> MinBookPrice();
    }
}
