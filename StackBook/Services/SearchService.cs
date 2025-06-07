using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewService _reviewService;
        public SearchService(IUnitOfWork unitOfWork, IReviewService reviewService)
        {
            _unitOfWork = unitOfWork;
            _reviewService = reviewService;
        }

        public async Task<List<BookRatingViewModel>> SearchBooksAsync(string query)
        {

            var books = await _unitOfWork.Book.GetAllAsync("Authors");
            if (string.IsNullOrEmpty(query))
            {
                var bookSearch = new List<BookRatingViewModel>();
                foreach (var book in books)
                {
                    bookSearch.Add(new BookRatingViewModel
                    {
                        Book = book,
                        AverageRating = await _reviewService.GetAverageRatingForBookAsync(book.BookId)
                    });
                }

            }
            var result = new List<BookRatingViewModel>();
            foreach (var book in books)
            {
                if (book.BookTitle.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    book.Authors.Any(a => a.AuthorName.Contains(query, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(new BookRatingViewModel
                    {
                        Book = book,
                        AverageRating = await _reviewService.GetAverageRatingForBookAsync(book.BookId)
                    });
                }
            }
            return result;
        }
    }
}