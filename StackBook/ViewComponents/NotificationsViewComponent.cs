using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;

namespace StackBook.Components
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationsViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return View(new List<Notification>());
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(Guid.Parse(claim.Value));
            return View(notifications?.OrderByDescending(n => n.CreatedAt).ToList() ?? new List<Notification>());
        }
    }

    public class NotificationCountViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationCountViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return View(0);
            }
            
            try
            {
                var count = await _notificationService.GetUnreadCountAsync(Guid.Parse(claim.Value));
                return View(count);
            }
            catch
            {
                return View(0); // Fallback
            }
        }
    }
}