using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtectionOfInfo.WebApp.Hubs
{
    public class ChatUser
    {
        private readonly List<ChatConnection> _connections;
        public string UserName { get; set; } = null!;
        public ChatUser(String userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            _connections = new List<ChatConnection>();
        }

        public IEnumerable<ChatConnection> Connections => _connections;
        public DateTime? ConnectedAt
        {
            get
            {
                if (Connections.Any())
                {
                    return Connections
                        .OrderByDescending(x => x.ConnectedAt)
                        .Select(x => x.ConnectedAt)
                        .First();
                }

                return null;
            }
        }

        public void AppendConnection(string connectedId)
        {
            if(connectedId == null)
            {
                throw new ArgumentNullException(nameof(connectedId));
            }

            var connection = new ChatConnection
            {
                ConnectedAt = DateTime.UtcNow,
                ConnectionId = connectedId
            };

            _connections.Add(connection);
        }

        public void RemoveConnection(string connectionId)
        {
            if(connectionId == null)
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            var connection = _connections.SingleOrDefault(x => x.ConnectionId.Equals(connectionId));
            if(connection == null)
            {
                return;
            }

            _connections.Remove(connection);
        }
    }
}
