using System.Linq.Expressions;
using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface IUserRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task SaveAsync();
    }
}
