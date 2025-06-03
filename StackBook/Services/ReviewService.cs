using StackBook.DAL;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StackBook.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;

        public ReviewService(IReviewRepository reviewRepository, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
        }
        public async Task<Review> CreateReviewFromOrderAsync(Guid orderId, Guid bookId, Guid userId, int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

            // Lấy đơn hàng
            var order = await _orderRepository.FindOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId)
                throw new InvalidOperationException("Order not found or not owned by user");
            //Kiểm tra đơn hàng đã hoàn thành
            if (order.Status != 4) // Giả sử 3 là trạng thái "Hoàn thành"
                throw new InvalidOperationException("Order is not completed");
            // Kiểm tra sách có nằm trong đơn hàng không
            var containsBook = order.OrderDetails.Any(i => i.BookId == bookId);
            if (!containsBook)
                throw new InvalidOperationException("Book is not part of this order");

            // Tạo review
            var review = new Review
            {
                ReviewId = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                OrderId = orderId,
                Rating = rating,
                Comment = comment
            };

            return await _reviewRepository.AddAsync(review);
        }

        public async Task<Review> GetReviewByIdAsync(Guid id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetAllAsync();
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            return await _reviewRepository.GetByUserIdAsync(userId);
        }

        public async Task<List<Review>> GetReviewsByBookIdAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                throw new ArgumentException("Book ID cannot be empty", nameof(bookId));
            }

            return await _reviewRepository.GetByBookIdAsync(bookId);
        }

        public async Task<List<Review>> GetReviewsByRatingAsync(int minRating, int? maxRating = null)
        {
            if (minRating < 1 || minRating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(minRating), "Rating must be between 1 and 5");
            }

            if (maxRating.HasValue && (maxRating < 1 || maxRating > 5))
            {
                throw new ArgumentOutOfRangeException(nameof(maxRating), "Rating must be between 1 and 5");
            }

            if (maxRating.HasValue && maxRating < minRating)
            {
                throw new ArgumentException("Max rating cannot be less than min rating");
            }

            return await _reviewRepository.GetByRatingAsync(minRating, maxRating);
        }

        public async Task<double> GetAverageRatingForBookAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                throw new ArgumentException("Book ID cannot be empty", nameof(bookId));
            }

            return await _reviewRepository.GetAverageRatingForBookAsync(bookId);
        }

        public async Task<int> GetReviewCountForBookAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                throw new ArgumentException("Book ID cannot be empty", nameof(bookId));
            }

            return await _reviewRepository.GetReviewCountForBookAsync(bookId);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            // if (await _reviewRepository.HasUserReviewedBookAsync(review.UserId, review.BookId))
            // {
            //     throw new InvalidOperationException("User has already reviewed this book");
            // }

            if (review.Rating < 1 || review.Rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(review.Rating), "Rating must be between 1 and 5");
            }

            return await _reviewRepository.AddAsync(review);
        }

        public async Task UpdateReviewAsync(Review review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            if (review.Rating < 1 || review.Rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(review.Rating), "Rating must be between 1 and 5");
            }

            await _reviewRepository.UpdateAsync(review);
        }

        public async Task DeleteReviewAsync(Guid id)
        {
            await _reviewRepository.DeleteAsync(id);
        }

        public async Task<bool> ReviewExistsAsync(Guid id)
        {
            return await _reviewRepository.ExistsAsync(id);
        }

        public async Task<bool> HasUserReviewedBookAsync(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            if (bookId == Guid.Empty)
            {
                throw new ArgumentException("Book ID cannot be empty", nameof(bookId));
            }

            return await _reviewRepository.HasUserReviewedBookAsync(userId, bookId);
        }
    }
}