using Calabonga.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CryptographyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Data
{
    //класс создан, чтоб сдать лабу по защите информации, полной реализации шифрования нет (записи и чтения)
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }

    public class User
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Roles { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string BlockedUser { get; set; } = null!;
        public string PasswordValidation { get; set; } = null!;
        public string FirstAccess { get; set; } = null!;
    }

    public interface IMyUserRepository
    {
        public Task<OperationResult<List<User>>> GetListAsync();
        public Task<OperationResult<User>> CreateAsync(User model);
        public Task<OperationResult<User>> DeleteAsync(string Id);
        public Task<OperationResult<User>> GetByIdAsync(string Id);
        public Task<OperationResult<User>> UpdateAsync(User model);
    }

    public class UserRepository : IMyUserRepository
    {
        public string EntityName { get; } = "User";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _repository;
        private readonly CancellationToken _cancellationToken;
        public UserRepository(IUnitOfWork<UserDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.GetRepository<User>();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = cancelTokenSource.Token;
        }

        public async Task<OperationResult<User>> CreateAsync(User model)
        {
            var operation = OperationResult.CreateResult<User>();
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            User entity = new User()
            {
                Id = model.Id,
                Roles = model.Roles,
                UserName = model.UserName,
                Password = model.Password,
                BlockedUser = model.BlockedUser,
                FirstAccess = model.FirstAccess,
                PasswordValidation = model.PasswordValidation
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

        public async Task<OperationResult<User>> DeleteAsync(string Id)
        {
            var operation = OperationResult.CreateResult<User>();

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

        public async Task<OperationResult<User>> GetByIdAsync(string Id)
        {
            var operation = OperationResult.CreateResult<User>();

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

        public async Task<OperationResult<User>> UpdateAsync(User model)
        {
            var operation = OperationResult.CreateResult<User>();

            var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == model.Id);
            if (entity == null)
            {
                operation.AddError($"Запись с Id = {model.Id} не найдена в БД");
                return operation;
            }

            entity.Password = model.Password;
            entity.BlockedUser = model.BlockedUser;
            entity.FirstAccess = model.FirstAccess;
            entity.PasswordValidation = model.PasswordValidation;

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

        public async Task<OperationResult<List<User>>> GetListAsync()
        {
            var operation = OperationResult.CreateResult<List<User>>();

            IList<User> authors = await _repository.GetAllAsync(
                orderBy: o => o.OrderBy(x => x.UserName)
                );

            operation.AddSuccess($"Список всех сущностей из таблицы {EntityName} получен");
            operation.Result = (List<User>)authors;
            return operation;
        }
    }

    public static class UserDataInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            const string userName = "admin";
            const string password = "123qwe123";

            var scope = serviceProvider.CreateScope();
            await using var contextUser = scope.ServiceProvider.GetService<UserDbContext>();

            var isExistsUser = contextUser!.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreatorUser && await databaseCreatorUser.ExistsAsync();
            if (isExistsUser) return;

            await contextUser!.Database.MigrateAsync();

            var userManager = scope.ServiceProvider.GetService<UserManager<MyIdentityUser>>();
            var userRepository = scope.ServiceProvider.GetService<IMyUserRepository>();
            if (userManager is null || userRepository is null)
            {
                throw new Exception("UserManager or RoleManager not registered");
            }

            var userSearch = await userManager.FindByNameAsync(userName);
            if(userSearch is null)
            {
                throw new Exception("UserManager or RoleManager not registered");
            }

            DataEncryptionService dataEncryption = new DataEncryptionService();

            if ((await userRepository.GetByIdAsync(dataEncryption.Encrypt_Aes(userSearch.Id))).Ok) return;

            var user = new User
            {
                Id = userSearch.Id,
                UserName = userName,
                Password = password,
                Roles = AppData.AdministratorRoleName + "/" + AppData.UserRoleName,
                PasswordValidation = false.ToString(),
                BlockedUser = false.ToString(),
                FirstAccess = true.ToString()
            };

            user.Id = dataEncryption.Encrypt_Aes(user.Id);
            user.UserName = dataEncryption.Encrypt_Aes(user.UserName);
            user.Password = dataEncryption.Encrypt_Aes(user.Password);
            user.Roles = dataEncryption.Encrypt_Aes(user.Roles);
            user.PasswordValidation = dataEncryption.Encrypt_Aes(user.PasswordValidation);
            user.BlockedUser = dataEncryption.Encrypt_Aes(user.BlockedUser);
            user.FirstAccess = dataEncryption.Encrypt_Aes(user.FirstAccess);

            await contextUser.AddAsync(user);

            await contextUser.SaveChangesAsync();
        }
    }
}
