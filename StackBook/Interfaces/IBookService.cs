using StackBook.Models;
using StackBook.ViewModels;
namespace StackBook.Interfaces
{
    public interface IBookService
    {
        Task UpdateBookQuantity(Guid bookId, int quantity, int status);
        Task<Book> GetByIdAsync(Guid bookId);
        Task UpdateAsync(Book book);
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> GetBooksByCategoryIdAsync(Guid categoryId);
        Task<List<Book>> GetBooksByAuthorIdAsync(Guid authorId);
        Task<List<BookRatingViewModel>> GetBookNewReleasesAsync(int top);
    }
}