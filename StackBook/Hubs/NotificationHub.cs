using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<Guid, string> _userConnections = new();

        public async Task JoinNotificationGroup(Guid userId)
        {
            ValidateUserId(userId);

            _userConnections.AddOrUpdate(userId, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            await Clients.Caller.SendAsync("GroupJoined", userId);
        }

        public async Task LeaveNotificationGroup(Guid userId)
        {
            ValidateUserId(userId);

            if (_userConnections.TryRemove(userId, out _))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
                await Clients.Caller.SendAsync("GroupLeft", userId);
            }
        }

        public async Task SendNotification(Guid userId, string message)
        {
            ValidateUserId(userId);
            ValidateMessage(message);

            await Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.UserIdentifier != null && Guid.TryParse(Context.UserIdentifier, out var userId))
            {
                await JoinNotificationGroup(userId);
            }

            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

            if (userId != Guid.Empty)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
                _userConnections.TryRemove(userId, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToAll(string message)
        {
            ValidateMessage(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToUser(Guid userId, string message)
        {
            ValidateUserId(userId);
            ValidateMessage(message);

            await Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message);
        }

        private void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty");
            }
        }

        private void ValidateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be empty or whitespace");
            }
        }
    }
}
