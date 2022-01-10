using AutoMapper;
using Calabonga.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Managers.InterfaceManager;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Repositories.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Managers
{
    public class BookManager : IBookManager
    {
        public string EntityName { get; } = "Book";
        private readonly IUnitOfWork<CatalogDbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMyRepository<Book> _bookMyRepository;
        private readonly IMyRepository<Category> _categoryMyRepository;
        private readonly IMyRepository<Publisher> _publisherMyRepository;
        private readonly IMyRepository<Author> _authorMyRepository;

        public BookManager(
            IUnitOfWork<CatalogDbContext> unitOfWork,
            IMapper mapper,
            IMyRepository<Book> bookRepository,
            IMyRepository<Category> categoryRepository,
            IMyRepository<Publisher> publisherRepository,
            IMyRepository<Author> authorRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookMyRepository = bookRepository;
            _publisherMyRepository = publisherRepository;
            _categoryMyRepository = categoryRepository;
            _authorMyRepository = authorRepository;
        }

        public async Task<OperationResult<Book>> GetByIdWithRelatedEntitiesAsync(Guid Id)
        {
            var repository = _unitOfWork.GetRepository<Book>();

            var operation = OperationResult.CreateResult<Book>();

            var entity = await repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == Id,
                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author)
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

        public async Task<OperationResult<List<Book>>> GetListWithRelatedEntitiesAsync()
        {
            var repository = _unitOfWork.GetRepository<Book>();

            var operation = OperationResult.CreateResult<List<Book>>();

            IList<Book> books = await repository.GetAllAsync(
                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author)
                );

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Book>)books;
            return operation;
        }

        public async Task<OperationResult<TEntity>> GetEmptyEntitiesWithParamAsync<TEntity>(TEntity entity) where TEntity : IBookParamViewModel
        {
            var operation = OperationResult.CreateResult<TEntity>(entity);

            var authors = await _authorMyRepository.GetListAsync();
            var publishers = await _publisherMyRepository.GetListAsync();
            var categories = await _categoryMyRepository.GetListAsync();

            if (authors.Ok)
            {
                operation.AppendLog($"Список всех сущностей из таблицы 'Author' получен");
                entity.Authors = _mapper.Map<List<AuthorViewModel>>(authors.Result);
            }
            else
            {
                operation.AppendLog(authors.Logs);
                operation.MetaData = authors.MetaData;
                operation.Exception = authors.Exception;
                operation.Result = default(TEntity);
                return operation;
            }

            if (publishers.Ok)
            {
                operation.AppendLog($"Список всех сущностей из таблицы 'Publisher' получен");
                entity.Publishers = _mapper.Map<List<PublisherViewModel>>(publishers.Result);
            }
            else
            {
                operation.AppendLog(publishers.Logs);
                operation.MetaData = publishers.MetaData;
                operation.Exception = publishers.Exception;
                operation.Result = default(TEntity);
                return operation;
            }

            if (categories.Ok)
            {
                operation.AppendLog($"Список всех сущностей из таблицы 'Category' получен");
                entity.Categories = _mapper.Map<List<CategoryViewModel>>(categories.Result);
            }
            else
            {
                operation.AppendLog(categories.Logs);
                operation.MetaData = categories.MetaData;
                operation.Exception = categories.Exception;
                operation.Result = default(TEntity);
                return operation;
            }

            operation.Result = entity;
            operation.AddSuccess($"Списки сущностей из таблиц {_authorMyRepository.EntityName}, {_categoryMyRepository.EntityName}, {_publisherMyRepository.EntityName} добавлены");
            return operation;
        }

        public async Task<OperationResult<TEntity>> СheckRelationshipsWithOtherEntitiesAsync<TEntity>(TEntity entity) where TEntity : IBookParamViewModel
        {
            var operation = OperationResult.CreateResult<TEntity>(entity);

            var author = await _authorMyRepository.GetByIdAsync((Guid)entity.AuthorId!);
            if (!author.Ok)
            {
                operation.AppendLog(author.Logs);
                operation.MetaData = author.MetaData;
                operation.Exception = author.Exception;
                operation.Result = default(TEntity);
                return operation;
            }

            var category = await _categoryMyRepository.GetByIdAsync((Guid)entity.CategoryId!);
            if (!category.Ok)
            {
                operation.AppendLog(category.Logs);
                operation.MetaData = category.MetaData;
                operation.Exception = category.Exception;
                operation.Result = default(TEntity);
                return operation;
            }

            var publisher = await _publisherMyRepository.GetByIdAsync((Guid)entity.PublisherId!);
            if (!publisher.Ok)
            {
                operation.AppendLog(publisher.Logs);
                operation.MetaData = publisher.MetaData;
                operation.Exception = publisher.Exception;
                operation.Result = default(TEntity);
                return operation;
            }

            return operation;
        }

        public async Task<OperationResult<List<Book>>> SearchAsync(string ObjectSearch, string Search)
        {
            var _repository = _unitOfWork.GetRepository<Book>();

            IList<Book> categories = await _repository.GetAllAsync(true);
            switch (ObjectSearch.ToLower())
            {
                case "all":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "name":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "price":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase));
                    categories = categories.Where(x => x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    //categories = await _repository.GetAllAsync(true);
                    break;
            }

            var operation = OperationResult.CreateResult<List<Book>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Book>)categories;
            return operation;
        }

        public async Task<OperationResult<List<Book>>> SortAsync(string ObjectSort, string TypeSort)
        {
            var _repository = _unitOfWork.GetRepository<Book>();

            IList<Book> categories;
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
                case "price":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderBy(x => x.Price));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(orderBy: o => o.OrderByDescending(x => x.Price));
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

            var operation = OperationResult.CreateResult<List<Book>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Book>)categories;
            return operation;
        }

        public async Task<OperationResult<List<Book>>> SearchWithOtherEntitiesAsync(string ObjectSearch, string Search)
        {
            var _repository = _unitOfWork.GetRepository<Book>();

            IList<Book> categories = await _repository.GetAllAsync(
                        include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
            switch (ObjectSearch.ToLower())
            {
                case "all":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Category!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Publisher!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    //    x.Author!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase),
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    categories = categories.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Category!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Publisher!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        x.Author!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "name":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase),
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    categories = categories.Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "price":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase),
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    categories = categories.Where(x => x.Price.ToString().Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "publishername":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Publisher!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase),
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    categories = categories.Where(x => x.Publisher!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "categoryname":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Category!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase),
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    categories = categories.Where(x => x.Category!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "authorname":
                    //categories = await _repository.GetAllAsync(
                    //    predicate: x => x.Author!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase),
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    categories = categories.Where(x => x.Author!.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    //categories = await _repository.GetAllAsync(
                    //    include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    break;
            }

            var operation = OperationResult.CreateResult<List<Book>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Book>)categories;
            return operation;
        }

        public async Task<OperationResult<List<Book>>> SortWithOtherEntitiesAsync(string ObjectSort, string TypeSort)
        {
            var _repository = _unitOfWork.GetRepository<Book>();

            IList<Book> categories;
            switch (ObjectSort.ToLower())
            {
                case "name":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderBy(x => x.Name));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderByDescending(x => x.Name));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                            break;
                    }
                    break;
                case "price":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderBy(x => x.Price));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderByDescending(x => x.Price));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                            break;
                    }
                    break;
                case "publishername":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderBy(x => x.Publisher!.Name));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderByDescending(x => x.Publisher!.Name));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                            break;
                    }
                    break;
                case "categoryname":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderBy(x => x.Category!.Name));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderByDescending(x => x.Category!.Name));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                            break;
                    }
                    break;
                case "authorname":
                    switch (TypeSort.ToLower())
                    {
                        case "orderby":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderBy(x => x.Author!.Name));
                            break;
                        case "orderbydescending":
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author),
                                orderBy: o => o.OrderByDescending(x => x.Author!.Name));
                            break;
                        default:
                            categories = await _repository.GetAllAsync(
                                include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                            break;
                    }
                    break;
                default:
                    categories = await _repository.GetAllAsync(
                        include: i => i.Include(x => x.Publisher).Include(x => x.Category).Include(x => x.Author));
                    break;
            }

            var operation = OperationResult.CreateResult<List<Book>>();
            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<Book>)categories;
            return operation;
        }

        public async Task<OperationResult<decimal>> MaxBookPrice()
        {
            var maxPrice = await _unitOfWork.GetRepository<Book>().MaxAsync(x => x.Price);
            var operation = OperationResult.CreateResult<decimal>(maxPrice);
            operation.AddSuccess($"Максимальное значение в таблице {EntityName} найдено");
            return operation;
        }
        public async Task<OperationResult<decimal>> MinBookPrice()
        {
            var minPrice = await _unitOfWork.GetRepository<Book>().MinAsync(x => x.Price);
            var operation = OperationResult.CreateResult<decimal>(minPrice);
            operation.AddSuccess($"Максимальное значение в таблице {EntityName} найдено");
            return operation;
        }
    }
}
