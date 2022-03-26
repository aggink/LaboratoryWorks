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
    public class PublisherManager : IMyManager<Publisher>
    {
        public string EntityName { get; } = "Publisher";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IRepository<Publisher> _repository;

        public PublisherManager(IUnitOfWork<CatalogDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.GetRepository<Publisher>();
        }

        public async Task<OperationResult<Publisher>> GetByIdWithRelatedEntitiesAsync(Guid Id)
        {
            var repository = _unitOfWork.GetRepository<Publisher>();

            var operation = OperationResult.CreateResult<Publisher>();

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

        public async Task<OperationResult<List<Publisher>>> GetListWithRelatedEntitiesAsync()
        {
            var repository = _unitOfWork.GetRepository<Publisher>();

            var operation = OperationResult.CreateResult<List<Publisher>>();

            IList<Publisher> publishers = await repository.GetAllAsync(
                include: i => i.Include(x => x.Books!)
                );

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Publisher>)publishers;
            return operation;
        }

        public async Task<OperationResult<List<Publisher>>> SearchAsync(string ObjectSearch, string Search)
        {
            IList<Publisher> categories = await _repository.GetAllAsync(true);
            switch (ObjectSearch.ToLower())
            {
                case "all":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Description.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(
                        x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Description.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "name":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(
                        x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "description":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Description.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(
                        x => x.Description.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    //categories = await _repository.GetAllAsync(true);
                    break;
            }

            var operation = OperationResult.CreateResult<List<Publisher>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Publisher>)categories;
            return operation;
        }

        public async Task<OperationResult<List<Publisher>>> SortAsync(string ObjectSort, string TypeSort)
        {
            IList<Publisher> categories;
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
                case "description":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderBy(x => x.Description));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderByDescending(x => x.Description));
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

            var operation = OperationResult.CreateResult<List<Publisher>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Publisher>)categories;
            return operation;
        }
    }
}
