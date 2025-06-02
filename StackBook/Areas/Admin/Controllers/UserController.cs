using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using X.PagedList;
using X.PagedList.Extensions;

namespace StackBook.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5; // S? user m?i trang
            int pageNumber = page ?? 1;

            var users = await _unitOfWork.User.GetAllAsync();
            var pagedUsers = users.OrderBy(u => u.UserId).ToPagedList(pageNumber, pageSize);

            return View(pagedUsers);
        }
    }
}