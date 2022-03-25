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
        private readonly ChatManager _chatManager;
        private const string _defaultGroupName = "General";

        private readonly IUnitOfWork<ChatDbContext> _unitOfWork;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly CancellationToken _cancellationToken;  

        public CommunicationHub(
            ChatManager chatManager, 
            IUnitOfWork<ChatDbContext> unitOfWork,
            UserManager<MyIdentityUser> userManager)
        {
            _chatManager = chatManager;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cancellationToken = new CancellationTokenSource().Token;
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
            var chatRepository = _unitOfWork.GetRepository<ChatMessage>();
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            var userName = Context.User!.Identity?.Name ?? "Anonymous";
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                await Clients.Caller.SendErrorAsync("Пользователь не найден");
                return;
            }

            var entity = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                CreatedBy = userName,
                UpdatedBy = userName,
                UserId = Guid.Parse(user.Id),
                Message = message,
                FileId = null
            };

            await chatRepository.InsertAsync(entity, _cancellationToken);

            await _unitOfWork.SaveChangesAsync();
            if (!_unitOfWork.LastSaveChangesResult.IsOk)
            {
                await transaction.RollbackAsync(_cancellationToken);
                await Clients.Caller.SendErrorAsync("Ошибка при отправке сообщения");
                return;
            }

            await transaction.CommitAsync(_cancellationToken);
            await Clients.All.SendMessageAsync(userName, message);
        }
    }
}
