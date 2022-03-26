using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using ProtectionOfInfo.WebApp.ViewModels.ChatViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Hubs
{
    public interface ICommunicationHub
    {
        Task SendMessageAsync(string userName, string message);
        Task UpdateUsersAsync(IEnumerable<string> users);
        Task SendImageAsync(string userName, string imageUrl);
        Task SendUrlAsync(string userName, string url, string fileName);
        Task SendErrorAsync(string errorMessage);
        Task SendAllMessagesAsync(List<MessageViewModel> messages);
    }
}