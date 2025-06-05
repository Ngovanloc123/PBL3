using StackBook.Models;
using StackBook.Data;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace StackBook.DAL.Repository
{
    public class NotificationsRepository: INotificationsRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Notification> SendNotificationAsync(Guid userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Status = false, // Chưa đọc
                CreatedAt = DateTime.UtcNow // Thời gian tạo thông báo
            };
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
        public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            return notification ?? throw new Exception("Notification not found");
        }
        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
            return notifications;
        }
        public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
        {
            var notifications = await _context.Notifications.Where(n => n.UserId == userId && !n.Status).ToListAsync();
            return notifications;
        }
        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await GetNotificationByIdAsync(notificationId);
            notification.Status = true; // Đánh dấu là đã đọc
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
        public async Task MarkAsUnreadAsync(Guid notificationId)
        {
            var notification = await GetNotificationByIdAsync(notificationId);
            notification.Status = false; // Đánh dấu là chưa đọc
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await GetUnreadNotificationsAsync(userId);
            foreach (var notification in notifications)
            {
                notification.Status = true; // Đánh dấu là đã đọc
            }
            _context.Notifications.UpdateRange(notifications);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await GetNotificationByIdAsync(notificationId);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var count = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.Status);
            return count;
        }
    }
}
