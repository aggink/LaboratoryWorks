using Calabonga.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Hubs
{
    [Authorize]
    public class CommunicationHub : Hub<ICommunicationHub>
    {
        private readonly IChatMessagesManager _chatMessagesManager;
        private readonly ChatManager _chatManager;
        private const string _defaultGroupName = "General";

        public CommunicationHub(
            ChatManager chatManager,
            IChatMessagesManager chatMessagesManager)
        {
            _chatManager = chatManager;
            _chatMessagesManager = chatMessagesManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            var connectionId = Context.ConnectionId;
            _chatManager.ConnectUser(userName, connectionId);
            await Groups.AddToGroupAsync(connectionId, _defaultGroupName);
            await UpdateUsersAsync();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var isUserRemoved = _chatManager.DisconnectUser(Context.ConnectionId);
            if (!isUserRemoved)
            {
                await base.OnDisconnectedAsync(exception);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _defaultGroupName);
            await UpdateUsersAsync();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateUsersAsync()
        {
            var users = _chatManager.Users.Select(x => x.UserName).ToList();
            await Clients.All.UpdateUsersAsync(users);
        }  

        public async Task SendMessageAsync(string message)
        {
            var userName = Context.User!.Identity?.Name ?? "Anonymous";
            try
            {
                await _chatMessagesManager.AddMessageAsync(userName, message);
                await Clients.All.SendMessageAsync(userName, message);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendErrorAsync(ex.Message);
            }          
        }

        public async Task SendAllMessagesAsync(string text)
        {
            try
            {
                var messages = await _chatMessagesManager.GetAllMessagesAsync();
                if(messages == null)
                {
                    throw new Exception("Ошибка при получении списка сообщений");
                }

                await Clients.Caller.SendAllMessagesAsync(messages);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendErrorAsync(ex.Message);
            }
        }
    }
}
