using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtectionOfInfo.WebApp.Hubs
{
    public class ChatManager
    {
        public List<ChatUser> Users { get; } = new();

        public void ConnectUser(string userName, string connectedId)
        {
            var userAlreadyExists = GetConnectedUserByName(userName);
            if(userAlreadyExists != null)
            {
                userAlreadyExists.AppendConnection(connectedId);
                return;
            }

            var user = new ChatUser(userName);
            user.AppendConnection(connectedId);
            Users.Add(user);
        }

        public bool DisconnectUser(string connectedId)
        {
            var userExists = GetConnectedUserById(connectedId);
            if(userExists == null)
            {
                return false;
            }

            if (!userExists.Connections.Any())
            {
                return false;
            }

            var connectionExists = userExists.Connections.Select(x => x.ConnectionId).First().Equals(connectedId);
            if (!connectionExists)
            {
                return false;
            }

            if(userExists.Connections.Count() == 1)
            {
                Users.Remove(userExists);
                return true;
            }

            userExists.RemoveConnection(connectedId);
            return false;
        }

        private ChatUser? GetConnectedUserById(string connectionId)
        {
            return Users
                .FirstOrDefault(x => x.Connections.Select(c => c.ConnectionId)
                .Contains(connectionId));
        }

        private ChatUser? GetConnectedUserByName(string userName)
        {
            return Users
                .FirstOrDefault(x => string.Equals(
                    x.UserName,
                    userName,
                    StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
