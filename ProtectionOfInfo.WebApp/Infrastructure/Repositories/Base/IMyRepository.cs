using Microsoft.EntityFrameworkCore.Storage;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base
{
    public interface IMyRepository<TEntity>
    {
        string EntityName { get; }
        Task<OperationResult<List<TEntity>>> GetListAsync();
        Task<OperationResult<TEntity>> GetByIdAsync(Guid Id);
        Task<OperationResult<TEntity>> CreateAsync(TEntity model);
        Task<OperationResult<TEntity>> UpdateAsync(TEntity model);
        Task<OperationResult<TEntity>> DeleteAsync(Guid Id);
    }
}
