using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.VMs;
using StackBook.Interfaces;
using StackBook.Middleware;
using StackBook.ViewModels;

namespace StackBook.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(IUnitOfWork unitOfWork, ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _UnitOfWork = unitOfWork;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var books = await _UnitOfWork.Book.GetAllAsync();
            return View(books);
        }

        //public async Task<IActionResult> Checkout() 
        //{
        //    var books = await _UnitOfWork.Book.GetAllAsync();
        //    return View(books);
        //}

        //[HttpPost]
        //public IActionResult AdVMCart(Guid bookId, int quantity = 1)
        //{
        //    TempData["Message"] = "Book added to cart successfully!";
        //    return View();
        //}

        // Phương thức tạo giỏ hàng
        //[HttpPost("create")]
        //[Authorize]
        //public async Task<IActionResult> CreateCart()
        //{
        //    try
        //    {
        //        var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
        //        if (userIdValue == null)
        //        {
        //            return View("Error", new { message = "User ID not found." });
        //        }

        //        Guid userId = Guid.Parse(userIdValue);
        //        await _cartService.CreateCartAsync(userId);

        //        // Trả về View với thông điệp thành công
        //        return View("Cart", new { message = "Cart created successfully", Success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error", new { message = "Error creating cart", error = ex.Message });
        //    }
        //}

        // Phương thức thêm sách vào giỏ hàng
        [HttpPost("add")]
        [Authorize]
        [AuthorizeRole("user")]
        public async Task<IActionResult> AdVMCart(BookInCartVM bookInCartVM)
        {
            try
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
                if (userIdValue == null)
                {
                    return View("Error", new { message = "User ID not found." });
                }

                Guid userId = Guid.Parse(userIdValue);
                await _cartService.AdVMCartAsync(userId, bookInCartVM.BookId, bookInCartVM.Quantity);

                return View("Cart", new { message = "Book added to cart successfully", Success = true });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error adding book to cart", error = ex.Message });
            }
        }

        // Phương thức cập nhật số lượng sách trong giỏ hàng
        [HttpPut("update/{userId}/{bookId}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuantityAsync(Guid userId, Guid bookId, [FromBody] int quantity)
        {
            try
            {
                await _cartService.UpdateQuantityAsync(userId, bookId, quantity);
                return View("Cart", new { message = "Quantity updated successfully", Success = true });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error updating quantity", error = ex.Message });
            }
        }

        // Phương thức xóa sách khỏi giỏ hàng
        [HttpDelete("remove/{userId}/{bookId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(Guid userId, Guid bookId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(userId, bookId);
                return View("Cart", new { message = "Book removed from cart successfully", Success = true });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error removing book from cart", error = ex.Message });
            }
        }

        // Phương thức xóa toàn bộ giỏ hàng
        [HttpDelete("clear/{userId}")]
        [Authorize]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            try
            {
                await _cartService.ClearCartAsync(userId);
                return View("Cart", new { message = "Cart cleared successfully", Success = true });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error clearing cart", error = ex.Message });
            }
        }

        // Phương thức xem tất cả sản phẩm trong giỏ hàng
        [HttpGet("details/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCartDetails(Guid userId)
        {
            try
            {
                var cartDetails = await _cartService.GetCartDetailsAsync(userId);
                return View("CartDetails", cartDetails);
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error retrieving cart details", error = ex.Message });
            }
        }

        // Phương thức tính tổng giá trị giỏ hàng
        [HttpGet("total/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetTotalPriceCart(Guid userId)
        {
            try
            {
                var totalPrice = await _cartService.GetTotalPriceCartAsync(userId);
                return View("Cart", new { message = "Total price retrieved successfully", Success = true, Data = totalPrice });
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error retrieving total price", error = ex.Message });
            }
        }
    }
}
