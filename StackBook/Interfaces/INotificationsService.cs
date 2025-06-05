using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackBook.Models;
using StackBook.DAL.IRepository;

namespace StackBook.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> SendNotificationAsync(Guid userId, string message);
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<Notification> GetNotificationByIdAsync(Guid notificationId);
        Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAsUnreadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task DeleteNotificationAsync(Guid notificationId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}