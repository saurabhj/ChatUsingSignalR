using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Website.Models;

namespace Website {
    public class ChatHub : Hub {

        #region Private Properties

        private static List<UserDetail> _ConnectedUsers;
        public static List<UserDetail> ConnectedUsers {
            get { return _ConnectedUsers ?? (_ConnectedUsers = new List<UserDetail>()); }
        }

        #endregion

        public void Connect(string username) {
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.Id == id) == 0) {
                ConnectedUsers.Add(new UserDetail { Id = id, Username = username });

                // Send list of connected users to the new user
                Clients.Caller.onConnected(id, username, ConnectedUsers);

                // Send information of new user to all existing users
                Clients.AllExcept(id).onNewUserConnected(id, username);
            }
        }

        public void SendToAll(string name, string message) {
            // Send message to all folks who are connected to the main chat room
            Clients.All.broadcastMessage(name, message);
        }

        public void SendPrivateMessage(string toUserId, string message) {
            var fromUserId = Context.ConnectionId;
            
            var toUser = ConnectedUsers.FirstOrDefault(x => x.Id == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.Id == fromUserId);

            if (toUser != null && fromUser != null) {
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.Username, message);
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.Username, message);
            }

        }

        public override Task OnDisconnected(bool stopCalled) {
            var item = ConnectedUsers.FirstOrDefault(x => x.Id == Context.ConnectionId);
            if (item != null) {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Username);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}