using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using Microsoft.EntityFrameworkCore;

namespace StackBook.DAL.Repository
{
    public class ReviewRepository : Repository<Review>,  IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Review> GetByIdAsync(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            return review ?? throw new Exception("Review not found");
        }

        public async Task<List<Review>> GetAllAsync()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return reviews;
        }
        public async Task<List<Review>> GetByUserIdAsync(Guid userId)
        {
            var reviews = await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
            return reviews;
        }
        public async Task<List<Review>> GetByBookIdAsync(Guid bookId)
        {
            var reviews = await _context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
            return reviews;

        }
        public async Task<List<Review>> GetByRatingAsync(int minRating, int? maxRating = null)
        {
            var reviews = await _context.Reviews.Where(r => r.Rating >= minRating && r.Rating <= maxRating).ToListAsync();
            return reviews;
        }
        public async Task<double?> GetAverageRatingForBookAsync(Guid bookId)
        {
            var averageRating = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();

            return averageRating;
        }

        public async Task<int> GetReviewCountForBookAsync(Guid bookId)
        {
            var reviewCount = await _context.Reviews.Where(r => r.BookId == bookId).CountAsync();
            return reviewCount;
        }
        public async Task<Review> AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }
        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var review = await GetByIdAsync(id);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Reviews.AnyAsync(r => r.ReviewId == id);
        }
        public async Task<bool> HasUserReviewedBookAsync(Guid userId, Guid bookId)
        {
            return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.BookId == bookId);
        }
        public async Task<bool> GetByUserIdBookIdOrderIdAsync(Guid userId, Guid bookId, Guid orderId)
        {
            // Console.WriteLine($"Searching for review with UserId: {userId}, BookId: {bookId}, OrderId: {orderId}");
            // var review = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId && r.OrderId == orderId);
            // Console.WriteLine($"Review found: {review.BookId}");
            // return review ?? throw new Exception("Review not found for the specified user, book, and order.");
            try
            {
                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.UserId == userId &&
                                            r.BookId == bookId &&
                                            r.OrderId == orderId);

                if (review == null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log lỗi
                throw new Exception(ex.Message); // Hoặc return null/giá trị mặc định
            }
        }
    }
}
