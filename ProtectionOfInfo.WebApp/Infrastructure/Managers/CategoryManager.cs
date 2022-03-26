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
    public class CategoryManager : IMyManager<Category>
    {
        public string EntityName { get; } = "Category";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IRepository<Category> _repository;

        public CategoryManager(IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.GetRepository<Category>();
        }

        public async Task<OperationResult<Category>> GetByIdWithRelatedEntitiesAsync(Guid Id)
        {
            var repository = _unitOfWork.GetRepository<Category>();

            var operation = OperationResult.CreateResult<Category>();

            var entity = await repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == Id,
                include: i => i.Include(x => x.Books!)
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

        public async Task<OperationResult<List<Category>>> GetListWithRelatedEntitiesAsync()
        {
            var repository = _unitOfWork.GetRepository<Category>();

            var operation = OperationResult.CreateResult<List<Category>>();

            IList<Category> categories = await repository.GetAllAsync(
                include: i => i.Include(x => x.Books!)
                );

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Category>)categories;
            return operation;
        }

        public async Task<OperationResult<List<Category>>> SearchAsync(string ObjectSearch, string Search)
        {
            IList<Category> categories = await _repository.GetAllAsync(true);
            switch (ObjectSearch.ToLower())
            {
                case "all":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Synopsis.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Synopsis.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "name":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "synopsis":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Synopsis.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(x => x.Synopsis.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    //categories = await _repository.GetAllAsync(true);
                    break;
            }

            var operation = OperationResult.CreateResult<List<Category>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Category>)categories;
            return operation;
        }

        public async Task<OperationResult<List<Category>>> SortAsync(string ObjectSort, string TypeSort)
        {
            IList<Category> categories;
            switch (ObjectSort.ToLower())
            {
                case "name":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderBy(x => x.Name));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderByDescending(x => x.Name));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(true);
                            break;
                    }
                    break;
                case "synopsis":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderBy(x => x.Synopsis));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderByDescending(x => x.Synopsis));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(true);
                            break;
                    }
                    break;
                default:
                    categories = await _repository.GetAllAsync(true);
                    break;
            }

            var operation = OperationResult.CreateResult<List<Category>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Category>)categories;
            return operation;
        }
    }
}
