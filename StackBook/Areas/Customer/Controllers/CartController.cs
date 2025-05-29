using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.VMs;
using StackBook.Interfaces;
using StackBook.Middleware;
using StackBook.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.RenderTree;
using StackBook.Models;
using StackBook.Utils;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;

namespace StackBook.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtUtils _jwtUtils;

        public CartController(IUnitOfWork unitOfWork, ICartService cartService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils)
        {
            _UnitOfWork = unitOfWork;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy userId từ claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    TempData["error"] = "User ID not found.";
                    return RedirectToAction("Index", "Home", new { area = "Site" });
                }

                var userId = Guid.Parse(userIdClaim);

                

                // Lấy danh sách sách trong giỏ hàng
                var cartDetails = await _cartService.GetCartDetailsAsync(userId);

                // Xóa CheckoutRequest trên Session
                if (HttpContext.Session.Keys.Contains("CheckoutRequest"))
                {
                    HttpContext.Session.Remove("CheckoutRequest");
                }

                return View(cartDetails);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error loading cart: {ex.Message}";
                return View("Error", new { message = "Error loading cart", error = ex.Message });
            }
        }

        //[HttpPost]
        public async Task<IActionResult> Checkout(List<SelectedBookVM> selectedBookVMs)
        {
            List<SelectedBookVM> selected;
            var userId = Request.Cookies["userId"];
            if (selectedBookVMs != null && selectedBookVMs.Any(b => b.IsSelected))
            {
                selected = selectedBookVMs.Where(b => b.IsSelected).ToList();

                
                var user = await _UnitOfWork.User.GetAsync(u => u.UserId == Guid.Parse(userId), "ShippingAddresses");

                var selectedBooks = new List<SelectedBook>();
                foreach (var s in selected)
                {
                    var book = await _UnitOfWork.Book.GetAsync(b => b.BookId == s.BookId);
                    selectedBooks.Add(new SelectedBook
                    {
                        Book = book,
                        Quantity = s.Quantity
                    });
                }

                // Lấy shippping address đầu tiên
                var shippingAddressDefault = await _UnitOfWork.ShippingAddress.GetAsync(sa => sa.UserId == Guid.Parse(userId));

                var checkoutRequestNew = new CheckoutRequest
                {
                    User = user,
                    SelectedBooks = selectedBooks,
                    shippingAddressDefault = shippingAddressDefault
                };

                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                HttpContext.Session.SetString("CheckoutRequest", JsonConvert.SerializeObject(checkoutRequestNew, jsonSettings));

                return View("Checkout", checkoutRequestNew);
            }

            var sessionData = HttpContext.Session.GetString("CheckoutRequest");
            if (string.IsNullOrEmpty(sessionData))
            {
                TempData["Error"] = "No books selected for checkout.";
                return RedirectToAction("Index", "Cart");
            }

            // Parse session thành CheckoutRequest
            var checkoutRequest = JsonConvert.DeserializeObject<CheckoutRequest>(sessionData);
            // Lấy lại user cùng với address để cập nhật lại address
            checkoutRequest.User = await _UnitOfWork.User.GetAsync(u => u.UserId == Guid.Parse(userId), "ShippingAddresses");

            return View("Checkout", checkoutRequest);
            

            // Handle the case where no books are selected
            //TempData["error"] = "No books selected for checkout.";
            //return RedirectToAction("Index");
        }





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
        //[AuthorizeRole("Customer")]
        public async Task<IActionResult> AddToCart(BookInCartVM bookInCartVM)
        {
            try
            {
                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine(currentUserIdClaims);
                if (currentUserIdClaims == null)
                {
                    TempData["error"] = "User ID not found.";
                    return RedirectToAction("Index", "Home", new { area = "Site" });
                }

                Guid userId = Guid.Parse(currentUserIdClaims);
                await _cartService.AddToCartAsync(userId, bookInCartVM.BookId, bookInCartVM.Quantity);
                // Trả về thông báo thành công
                Console.WriteLine("Book added to cart successfully!");
                TempData["success"] = "Book added to cart successfully!";
                // Lấy URL trang trước đó
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                    return Redirect(referer);
                var accessToken = Request.Cookies["accessToken"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    return RedirectToAction("Index", "Home", new { area = "Site" });
                }
                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)	
                });
                // Nếu không có referer, về trang chủ
                return RedirectToAction("Index", "Home", new { area = "Site" });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error adding book to cart: {ex.Message}";
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                    return Redirect(referer);

                return RedirectToAction("Index", "Home", new { area = "Site" });
            }
        }


        // Phương thức cập nhật số lượng sách trong giỏ hàng
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateQuantity(Guid userId, Guid bookId, int quantity)
        {
            try
            {
                if (quantity < 1)
                    quantity = 1;

                await _cartService.UpdateQuantityAsync(userId, bookId, quantity);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error updating quantity", error = ex.Message });
            }
        }


        // Phương thức xóa sách khỏi giỏ hàng
        [HttpPost("remove")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(Guid userId, Guid bookId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(userId, bookId);
                ViewData["success"] = "Book removed from cart successfully";
                var updatedCart = await _cartService.GetCartDetailsAsync(userId);

                return View("Index", updatedCart);
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
                TempData["success"] = "Cart cleared successfully";
                return RedirectToAction("Index");
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
                TempData["success"] = "Total price retrieved successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Error retrieving total price", error = ex.Message });
            }
        }
    }
}