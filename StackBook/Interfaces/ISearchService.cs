using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Interfaces
{
    public interface ISearchService
    {
        Task<List<BookRatingViewModel>> SearchBooksAsync(string query);
        Task<List<Book>> SearchBooksAdminAsync(string query);
    }
}