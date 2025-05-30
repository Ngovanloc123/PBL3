using System.Linq.Expressions;
using StackBook.Models;

namespace StackBook.DAL.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);

        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);


        Task AddAsync(T entity);

        Task DeleteAsync(T entity);

        Task DeleteRangeAsync(IEnumerable<T> entities);

        Task<T> UpdateAsync(T entity);
    }
}
