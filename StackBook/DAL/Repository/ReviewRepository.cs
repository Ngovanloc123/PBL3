using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using Microsoft.EntityFrameworkCore;

namespace StackBook.DAL.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
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
        public async Task<double> GetAverageRatingForBookAsync(Guid bookId)
        {
            var averageRating = await _context.Reviews.Where(r => r.BookId == bookId).AverageAsync(r => r.Rating);
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
    }
}
