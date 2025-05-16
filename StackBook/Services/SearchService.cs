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
        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Book>> SearchBooksAsync(string query)
        {

            var books = await _unitOfWork.Book.GetAllAsync("Authors");
            if (string.IsNullOrEmpty(query)) return books.ToList();
            var result = new List<Book>();
            foreach (var book in books)
            {
                // Thỏa mãn có chứa tên sách 
                bool matchedByTitle = book.BookTitle.ToLower().Contains(query.ToLower());

                // Thỏa mản có chứa Author  
                bool matchedByAuthor = book.Authors.Any(author =>
                    author.AuthorName.ToLower().Contains(query.ToLower())
                );

                if (matchedByTitle || matchedByAuthor)
                {
                    result.Add(book);
                }
            }
            return result;
        }
    }
}