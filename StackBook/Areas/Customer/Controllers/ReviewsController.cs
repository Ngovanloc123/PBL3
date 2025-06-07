using Microsoft.AspNetCore.Mvc;
using StackBook.Models;
using System.Security.Claims;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;

namespace StackBook.Controllers
{
    [Area("Customer")]
    [Route("Customer/Order/[controller]/[action]")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(Review review)
        {
            if (ModelState.IsValid)
            {
                // Get current user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }

                // Check if the user has already reviewed this book in this order
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.UserId == Guid.Parse(userId)
                    && r.BookId == review.BookId
                    && r.OrderId == review.OrderId);

                if (existingReview != null)
                {
                    TempData["ErrorMessage"] = "You have already reviewed this book from this order.";
                    return RedirectToAction("Index", "Order", new { status = 4 }); // Redirect to delivered orders
                }

                // Set review details
                review.UserId = Guid.Parse(userId);
                review.ReviewId = Guid.NewGuid();

                // Save review
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you for your review!";
                return RedirectToAction("Index", "Order", new { status = 4 }); // Redirect to delivered orders
            }

            // If we got this far, something failed
            TempData["ErrorMessage"] = "There was an error submitting your review.";
            return RedirectToAction("Index", "Order", new { status = 4 }); // Redirect to delivered orders
        }
    }
}