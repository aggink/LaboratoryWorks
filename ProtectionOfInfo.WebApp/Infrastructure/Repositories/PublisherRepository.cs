using Calabonga.UnitOfWork;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Repositories
{
    public class PublisherRepository : IMyRepository<Publisher>
    {
        public string EntityName { get; } = "Publisher";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IRepository<Publisher> _repository;
        private readonly CancellationToken _cancellationToken;

        public PublisherRepository(IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<Publisher>();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = cancelTokenSource.Token;
        }

        public async Task<OperationResult<Publisher>> CreateAsync(Publisher model)
        {
            var operation = OperationResult.CreateResult<Publisher>();
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            Publisher entity = new Publisher() {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
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

        public async Task<OperationResult<Publisher>> DeleteAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Publisher>();

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

        public async Task<OperationResult<Publisher>> GetByIdAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Publisher>();

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

        public async Task<OperationResult<Publisher>> UpdateAsync(Publisher model)
        {
            var operation = OperationResult.CreateResult<Publisher>();

            var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == model.Id);
            if (entity == null)
            {
                operation.AddError($"Запись с Id = {model.Id} не найдена в БД");
                return operation;
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
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

        public async Task<OperationResult<List<Publisher>>> GetListAsync()
        {
            var operation = OperationResult.CreateResult<List<Publisher>>();

            IList<Publisher> publishers = await _repository.GetAllAsync(true);

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Publisher>)publishers;
            return operation;
        }
    }
}
