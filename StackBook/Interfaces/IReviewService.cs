using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.Interfaces
{
    public interface IReviewService
    {
        Task<Review> GetReviewByIdAsync(Guid id);
        Task<List<Review>> GetAllReviewsAsync();
        Task<List<Review>> GetReviewsByUserIdAsync(Guid userId);
        Task<List<Review>> GetReviewsByBookIdAsync(Guid bookId);
        Task<List<Review>> GetReviewsByRatingAsync(int minRating, int? maxRating = null);
        Task<double> GetAverageRatingForBookAsync(Guid bookId);
        Task<int> GetReviewCountForBookAsync(Guid bookId);
        Task<Review> AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Guid id);
        Task<bool> ReviewExistsAsync(Guid id);
        Task<bool> HasUserReviewedBookAsync(Guid userId, Guid bookId);
    }
}