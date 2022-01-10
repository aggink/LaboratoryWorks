using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Managers.Base
{
    public interface IMyManager<TEntity>
    {
        string EntityName { get; }
        Task<OperationResult<List<TEntity>>> GetListWithRelatedEntitiesAsync();
        Task<OperationResult<TEntity>> GetByIdWithRelatedEntitiesAsync(Guid Id);
        Task<OperationResult<List<TEntity>>> SearchAsync(string ObjectSearch, string Search);
        Task<OperationResult<List<TEntity>>> SortAsync(string ObjectSort, string TypeSort);
    }
}
