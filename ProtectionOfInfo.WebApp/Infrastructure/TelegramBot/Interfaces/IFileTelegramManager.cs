using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using ProtectionOfInfo.WebApp.ViewModels.TelegramViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.TelegramBot.Interfaces
{
    public interface IFileTelegramManager
    {
        public Task<bool> AddFileAsync(File file, string Description, string Value, bool IsPublication);
        public Task<bool> UpdateFileAsync(Guid id, string description, string value, bool isPublication);
        public Task<bool> DeleteFileAsync(Guid id);
        public Task<List<FileTelegramViewModel>> GetAllFilesAsync();
        public Task<MyFileInfo> GetFileAsync(Guid id);
        public Task<FileTelegramViewModel> GetFileWithInfoAsync(Guid id);
    }
}
