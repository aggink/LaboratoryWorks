using Calabonga.UnitOfWork;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Repositories
{
    public class AuthorRepository : IMyRepository<Author>
    {
        public string EntityName { get; } = "Author";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IRepository<Author> _repository;
        private readonly CancellationToken _cancellationToken;
        public AuthorRepository(IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<Author>();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = cancelTokenSource.Token;
        }

        public async Task<OperationResult<Author>> CreateAsync(Author model)
        {
            var operation = OperationResult.CreateResult<Author>();
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            Author entity = new Author()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Biography = model.Biography,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy
            };

            await _repository.InsertAsync(entity, _cancellationToken);

            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                await transaction.RollbackAsync(_cancellationToken);
                operation.AddError(_unitOfWork.LastSaveChangesResult.Exception);
                return operation;
            }

            await transaction.CommitAsync(_cancellationToken);
            operation.AddSuccess($"{EntityName} id = {entity.Id} успешно создан");
            operation.Result = entity;
            return operation;
        }

        public async Task<OperationResult<Author>> DeleteAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Author>();

            var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == Id);
            if (entity == null)
            {
                operation.AddError($"Запись с Id = {Id} не найдена в БД");
                return operation;
            }

            _repository.Delete(entity);

            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                operation.AddError(_unitOfWork.LastSaveChangesResult.Exception);
                return operation;
            }

            operation.AddSuccess($"{EntityName} id = {Id} успешно удалён");
            operation.Result = entity;
            return operation;
        }

        public async Task<OperationResult<Author>> GetByIdAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Author>();

            var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == Id);
            if (entity == null)
            {
                operation.AddError($"Запись с Id = {Id} не найдена в БД");
                return operation;
            }

            operation.AddSuccess($"{EntityName} id = {Id} успешно найден");
            operation.Result = entity;
            return operation;
        }

        public async Task<OperationResult<Author>> UpdateAsync(Author model)
        {
            var operation = OperationResult.CreateResult<Author>();

            var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == model.Id);
            if (entity == null)
            {
                operation.AddError($"Запись с Id = {model.Id} не найдена в БД");
                return operation;
            }

            entity.Name = model.Name;
            entity.Biography = model.Biography;
            entity.UpdatedBy = model.UpdatedBy;

            _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                operation.AddError(_unitOfWork.LastSaveChangesResult.Exception);
                return operation;
            }

            operation.AddSuccess($"{EntityName} id = {entity.Id} успешно обновлен");
            operation.Result = entity;
            return operation;
        }

        public async Task<OperationResult<List<Author>>> GetListAsync()
        {
            var operation = OperationResult.CreateResult<List<Author>>();

            IList<Author> authors = await _repository.GetAllAsync(true);

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Author>)authors;
            return operation;
        }
    }
}
