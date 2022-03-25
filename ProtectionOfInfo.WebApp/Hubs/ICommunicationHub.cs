using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
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
    }
}