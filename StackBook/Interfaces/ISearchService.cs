using StackBook.Models;

namespace StackBook.Interfaces
{
    public interface ISearchService
    {
        Task<List<Book>> SearchBooksAsync(string query);
    }
}
