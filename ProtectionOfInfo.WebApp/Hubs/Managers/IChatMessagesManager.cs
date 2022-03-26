using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using ProtectionOfInfo.WebApp.Data.Entities.CryptographyEntities;
using ProtectionOfInfo.WebApp.ViewModels.ChatViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Hubs
{
    public interface IChatMessagesManager
    {
        public Task<bool> AddMessageAsync(string userName, string message);
        public Task<List<MessageViewModel>> GetAllMessagesAsync();
        public Task<FileUrlViewModel> AddFileAsync(string userName, FileDescription file);
        public Task<MyFileInfo> GetFileAsync(string id);
        public Task<MyFileInfo> GetImageAsync(string id);
        public Task<bool> DeleteMessageAsync();
    }
}
