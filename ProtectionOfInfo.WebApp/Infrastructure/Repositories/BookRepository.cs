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
    public class BookRepository : IMyRepository<Book>
    {
        public string EntityName { get; } = "Book";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IRepository<Book> _repository;
        private readonly CancellationToken _cancellationToken;
        public BookRepository(IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<Book>();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = cancelTokenSource.Token;
        }

        public async Task<OperationResult<Book>> CreateAsync(Book model)
        {
            var operation = OperationResult.CreateResult<Book>();
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            Book entity = new Book()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Price = model.Price,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                PublisherId = model.PublisherId,
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

        public async Task<OperationResult<Book>> DeleteAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Book>();

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

        public async Task<OperationResult<Book>> GetByIdAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Book>();

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

        public async Task<OperationResult<Book>> UpdateAsync(Book model)
        {
            var operation = OperationResult.CreateResult<Book>();

            var entity = await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == model.Id);
            if (entity == null)
            {
                operation.AddError($"Запись с Id = {model.Id} не найдена в БД");
                return operation;
            }

            entity.Name = model.Name;
            entity.Price = model.Price;
            entity.AuthorId = model.AuthorId;
            entity.CategoryId = model.CategoryId;
            entity.PublisherId = model.PublisherId;
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

        public async Task<OperationResult<List<Book>>> GetListAsync()
        {
            var operation = OperationResult.CreateResult<List<Book>>();

            IList<Book> books = await _repository.GetAllAsync(true);

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Book>)books;
            return operation;
        }
    }
}
