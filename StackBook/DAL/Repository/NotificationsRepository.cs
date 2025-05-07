using StackBook.Models;
using StackBook.Data;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace StackBook.DAL
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
    }
}
