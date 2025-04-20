
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;
using static StackBook.ViewModels.BookWithAuthors;

namespace StackBook.DAL.IRepository
{
    public interface IBookRepository :IRepository<Book>
    {
        // 
        IEnumerable<BookDetailViewModel> GetAllBookDetails();
        List<BookWithAuthors> GetAllBookWithAuthor();
        BookWithAuthors GetBookWithAuthor(Guid? bookId);
        BookDetailViewModel? GetBookDetail(Guid? bookId);
        void UpdateBookDetail(BookDetailViewModel obj);
        void AddBookDetail(BookDetailViewModel bookAdd);
        void DeleteBookDetail(BookDetailViewModel bookDelete);
    }
}
