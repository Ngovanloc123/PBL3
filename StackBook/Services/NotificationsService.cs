using StackBook.DAL;
using StackBook.Hubs;
using StackBook.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;

namespace StackBook.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            INotificationsRepository notificationsRepository,
            IHubContext<NotificationHub> hubContext)
        {
            _notificationsRepository = notificationsRepository;
            _hubContext = hubContext;
        }

        public async Task<Notification> SendNotificationAsync(Guid userId, string message)
        {
            var notification = await _notificationsRepository.SendNotificationAsync(userId, message);
            
            // Gửi thông báo real-time
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("ReceiveNotification", notification);
                
            // Cập nhật số lượng thông báo chưa đọc
            var unreadCount = await _notificationsRepository.GetUnreadCountAsync(userId);
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("UpdateUnreadCount", unreadCount);

            return notification;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await _notificationsRepository.GetUserNotificationsAsync(userId);
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
        {
            return await _notificationsRepository.GetUnreadNotificationsAsync(userId);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await _notificationsRepository.MarkAsReadAsync(notificationId);
            var notification = await _notificationsRepository.GetNotificationByIdAsync(notificationId);
            
            // Cập nhật số lượng thông báo chưa đọc
            var unreadCount = await _notificationsRepository.GetUnreadCountAsync(notification.UserId);
            await _hubContext.Clients.Group(notification.UserId.ToString())
                .SendAsync("UpdateUnreadCount", unreadCount);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationsRepository.MarkAllAsReadAsync(userId);
            
            // Cập nhật số lượng thông báo chưa đọc
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("UpdateUnreadCount", 0);
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _notificationsRepository.GetNotificationByIdAsync(notificationId);
            await _notificationsRepository.DeleteNotificationAsync(notificationId);
            
            // Cập nhật số lượng thông báo chưa đọc
            var unreadCount = await _notificationsRepository.GetUnreadCountAsync(notification.UserId);
            await _hubContext.Clients.Group(notification.UserId.ToString())
                .SendAsync("UpdateUnreadCount", unreadCount);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _notificationsRepository.GetUnreadCountAsync(userId);
        }
    }
}