using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace StackBook.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinNotificationGroup(Guid userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        }

        public async Task LeaveNotificationGroup(Guid userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        }
    }
}