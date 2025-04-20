using System.Linq.Expressions;

namespace StackBook.DAL.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        // void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        
    }
}
