
using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface IBookRepository :IRepository<Book>
    {
        void Update(Book obj);
    }
}
