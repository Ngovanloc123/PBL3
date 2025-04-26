using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class SearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Book> SearchBooks(String query)
        {
            var books = _unitOfWork.Book.GetAll();
            var result = new List<Book>();
            foreach (var book in books)
            {
                // Thỏa mãn có chứa tên sách
                bool matchedByTitle = book.BookTitle.ToLower().Contains(query.ToLower());

                // // Thỏa mản có chứa Author
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
