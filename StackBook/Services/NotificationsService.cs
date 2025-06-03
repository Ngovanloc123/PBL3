using StackBook.DAL;
using StackBook.Models;
using System;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;

namespace StackBook.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationsRepository _notificationsRepository;

        public NotificationService(
            INotificationsRepository notificationsRepository)
        {
            _notificationsRepository = notificationsRepository;
        }

        public async Task<Notification> SendNotificationAsync(Guid userId, string message)
        {
            // gửi thông báo bình thường
            var notification = await _notificationsRepository.SendNotificationAsync(userId, message);
            // không dùng SignIr 
            // vì không cần cập nhật số lượng thông báo chưa đọc ngay lập tức
            return notification;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _notificationsRepository.GetUserNotificationsAsync(userId);
            notifications = notifications.OrderByDescending(n => n.CreatedAt).ToList();
            // Sắp xếp thông báo theo thời gian tạo mới nhất
            return notifications;
        }
        public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
        {
            return await _notificationsRepository.GetNotificationByIdAsync(notificationId);
        }
        public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
        {
            return await _notificationsRepository.GetUnreadNotificationsAsync(userId);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await _notificationsRepository.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationsRepository.MarkAllAsReadAsync(userId);
    
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _notificationsRepository.GetNotificationByIdAsync(notificationId);
            await _notificationsRepository.DeleteNotificationAsync(notificationId);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _notificationsRepository.GetUnreadCountAsync(userId);
        }
    }
}