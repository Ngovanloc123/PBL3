
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;
using static StackBook.ViewModels.BookWithAuthors;

namespace StackBook.DAL.IRepository
{
    public interface IBookRepository : IRepository<Book>
    {
        //void Update(Book entity);
        //void Delete(Book book);
        //đếm số sách đã bán
        Task<int> CountBooksSoldAsync(Guid bookId);
    }
}