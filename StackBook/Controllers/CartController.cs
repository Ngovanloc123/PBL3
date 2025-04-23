using Microsoft.AspNetCore.Mvc;
using StackBook.DTOs;
using StackBook.Services;
using System.Threading.Tasks;
using StackBook.Middleware;
using Microsoft.AspNetCore.Authorization;
using StackBook.Interfaces;
using StackBook.Models;
namespace StackBook.Controllers{
    [Route("cart/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }
        // Phương thức tao giỏ hàng
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateCart()
        {
            try
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
                if (userIdValue == null)
                {
                    return BadRequest("User ID not found.");
                }
                Guid userId = Guid.Parse(userIdValue);
                await _cartService.CreateCartAsync(userId);
                return new ObjectResult(new
                {
                    Code = 200,
                    message = "Cart created successfully",
                    Success = true,
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error creating cart",
                    error = ex.Message
                });
            }
        }
        // Phương thức thêm sách vào giỏ hàng
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] BookInCartDto bookInCartDto)
        {
            try
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
                if (userIdValue == null)
                {
                    return BadRequest("User ID not found.");
                }
                Guid userId = Guid.Parse(userIdValue);
                await _cartService.AddToCartAsync(userId, bookInCartDto.BookId, bookInCartDto.Quantity);
                return new ObjectResult(new
                {
                    Code = 200,
                    message = "Book added to cart successfully",
                    Success = true,
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error adding book to cart",
                    error = ex.Message
                });
            }
        }
        //Phương thức cập nhật số lượng sách trong giỏ hàng
        [HttpPut("update/{userId}/{bookId}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuantityAsync(Guid userId, Guid bookId, [FromBody] int quantity)
        {
            try
            {
                await _cartService.UpdateQuantityAsync(userId, bookId, quantity);
                return new ObjectResult(new
                {
                    Code = 200,
                    message = "Quantity updated successfully",
                    Success = true,
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error updating quantity",
                    error = ex.Message
                });
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
                return new ObjectResult(new
                {
                    Code = 200,
                    message = "Book removed from cart successfully",
                    Success = true,
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error removing book from cart",
                    error = ex.Message
                });
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
                return new ObjectResult(new
                {
                    Code = 200,
                    message = "Cart cleared successfully",
                    Success = true,
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error clearing cart",
                    error = ex.Message
                });
            }
        }
        // Phương thức toan bo san pham trong gio hang
        [HttpGet("details/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCartDetails(Guid userId)
        {
            try
            {
                var cartDetails = await _cartService.GetCartDetailsAsync(userId);
                return Ok(new
                {
                    Code = 200,
                    message = "Cart details retrieved successfully",
                    Success = true,
                    Data = cartDetails
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error retrieving cart details",
                    error = ex.Message
                });
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
                return Ok(new
                {
                    Code = 200,
                    message = "Total price retrieved successfully",
                    Success = true,
                    Data = totalPrice
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error retrieving total price",
                    error = ex.Message
                });
            }
        }
    }
}