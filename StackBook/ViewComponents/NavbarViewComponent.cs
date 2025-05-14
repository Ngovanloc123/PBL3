using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;

namespace StackBook.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public NavbarViewComponent(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserIdClaims = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaims))
            {
                ViewBag.CartCount = 0;
            }
            else
            {
                var currentUserId = Guid.Parse(currentUserIdClaims);
                ViewBag.CartCount = await _cartService.GetCartCount(currentUserId);
            }

            return View();
        }
    }

}
