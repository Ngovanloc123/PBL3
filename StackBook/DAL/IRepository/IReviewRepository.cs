using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StackBook.Models;
using StackBook.Data;
using StackBook.DAL.IRepository;

namespace StackBook.DAL.IRepository
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(Guid id);
        Task<List<Review>> GetAllAsync();
        Task<List<Review>> GetByUserIdAsync(Guid userId);
        Task<List<Review>> GetByBookIdAsync(Guid bookId);
        Task<List<Review>> GetByRatingAsync(int minRating, int? maxRating = null);
        Task<double?> GetAverageRatingForBookAsync(Guid bookId);
        Task<int> GetReviewCountForBookAsync(Guid bookId);
        Task<Review> AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasUserReviewedBookAsync(Guid userId, Guid bookId);
    }
}
