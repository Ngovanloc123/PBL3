using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.Interfaces
{
    public interface IReviewService
    {
        Task<Review> CreateReviewFromOrderAsync(Guid orderId, Guid bookId, Guid userId, int rating, string? comment = null);
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
        Task<bool> GetReviewByUserIdBookIdOrderIdAsync(Guid userId, Guid bookId, Guid orderId);
    }
}