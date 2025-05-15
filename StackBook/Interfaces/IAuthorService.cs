using StackBook.Models;

namespace StackBook.Interfaces
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAuthorsByBookId(Guid bookId);
    }
}