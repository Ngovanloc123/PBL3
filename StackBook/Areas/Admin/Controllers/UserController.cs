using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public UserController(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5; // S? user m?i trang
            int pageNumber = page ?? 1;

            var users = await _unitOfWork.User.GetAllAsync();
            var pagedUsers = users.OrderBy(u => u.UserId).ToPagedList(pageNumber, pageSize);

            return View(pagedUsers);
        }
        //Mở khóa trang chi tiết người dùng
        [HttpGet("Unlock/{id}")]
        public async Task<IActionResult> Unlock(Guid id)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction("Index");
            }

            user.LockStatus = false; // Mở khóa người dùng
            // Cập nhật thời gian mở khóa
            user.DateLock = null;
            user.AmountOfTime = 0; // Reset số lần khóa
            await _unitOfWork.User.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            // Thông báo thành công
            await _notificationService.SendNotificationAsync(user.UserId, "Your account has been unlocked successfully.");
            TempData["success"] = "User unlocked successfully.";
            return RedirectToAction("Index");
        }
        //Khóa trang chi tiết người dùng
        [HttpGet("Lock/{id}")]
        public async Task<IActionResult> Lock(Guid id)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction("Index");
            }
            user.LockStatus = true; // Khóa người dùng
            user.DateLock = DateTime.UtcNow; // Cập nhật thời gian khóa
            user.AmountOfTime++; // Tăng số lần khóa
            await _unitOfWork.User.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            // Thông báo thành công
            await _notificationService.SendNotificationAsync(user.UserId, "Your account has been locked due to suspicious activity.");
            TempData["success"] = "User locked successfully.";
            return RedirectToAction("Index");
        }
    }
}