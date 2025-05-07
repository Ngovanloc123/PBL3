using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface INotificationsRepository
    {
        // Gửi thông báo mới
        Task<Notification> SendNotificationAsync(Guid userId, string message);
        
        // Lấy thông báo theo ID
        Task<Notification> GetNotificationByIdAsync(Guid notificationId);
        
        // Lấy tất cả thông báo của user
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        
        // Lấy thông báo chưa đọc
        Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId);
        
        // Đánh dấu đã đọc
        Task MarkAsReadAsync(Guid notificationId);
        
        // Đánh dấu tất cả đã đọc
        Task MarkAllAsReadAsync(Guid userId);
        
        // Xóa thông báo
        Task DeleteNotificationAsync(Guid notificationId);
        
        // Lấy số lượng thông báo chưa đọc
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}
