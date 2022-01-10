using Calabonga.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.Base;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Managers
{
    public class AuthorManager : IMyManager<Author>
    {
        public string EntityName { get; } = "Author";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IRepository<Author> _repository;

        public AuthorManager(IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.GetRepository<Author>();
        }
        public async Task<OperationResult<Author>> GetByIdWithRelatedEntitiesAsync(Guid Id)
        {
            var operation = OperationResult.CreateResult<Author>();

            var entity = await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == Id,
                include: i => i.Include(x => x.Books)
                );

            if (entity == null)
            {
                operation.AddError($"Запись с Id = {Id} не найдена в БД");
                return operation;
            }

            operation.AddSuccess($"{EntityName} id = {Id} успешно найден");
            operation.Result = entity;
            return operation;
        }

        public async Task<OperationResult<List<Author>>> GetListWithRelatedEntitiesAsync()
        {
            var operation = OperationResult.CreateResult<List<Author>>();

            IList<Author> authors = await _repository.GetAllAsync(
                include: i => i.Include(x => x.Books)
                );

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Author>)authors;
            return operation;
        }

        public async Task<OperationResult<List<Author>>> SearchAsync(string ObjectSearch, string Search)
        {
            IList<Author> authors = await _repository.GetAllAsync(true);
            switch (ObjectSearch.ToLower())
            {
                case "all":
                    //authors = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Biography.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    authors = authors.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Biography.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "name":
                    //authors = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    authors = authors.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "biography":
                    //authors = await _repository.GetAllAsync(
                    //    predicate: x => x.Biography.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    authors = authors.Where(x => x.Biography.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    //authors = await _repository.GetAllAsync(true);
                    break;
            }

            var operation = OperationResult.CreateResult<List<Author>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Author>)authors;
            return operation;
        }

        public async Task<OperationResult<List<Author>>> SortAsync(string ObjectSort, string TypeSort)
        {
            IList<Author> authors;
            switch (ObjectSort.ToLower())
            {
                case "name":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            authors = await _repository.GetAllAsync(orderBy: o => o.OrderBy(x => x.Name));
                            break;
                        case "orderbydescending":
                            authors = await _repository.GetAllAsync(orderBy: o => o.OrderByDescending(x => x.Name));
                            break;
                        default:
                            authors = await _repository.GetAllAsync(true);
                            break;
                    }
                    break;
                case "biography":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            authors = await _repository.GetAllAsync(orderBy: o => o.OrderBy(x => x.Biography));
                            break;
                        case "orderbydescending":
                            authors = await _repository.GetAllAsync(orderBy: o => o.OrderByDescending(x => x.Biography));
                            break;
                        default:
                            authors = await _repository.GetAllAsync(true);
                            break;
                    }
                    break;
                default:
                    authors = await _repository.GetAllAsync(true);
                    break;
            }

            var operation = OperationResult.CreateResult<List<Author>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Author>)authors;
            return operation;
        }
    }
}
