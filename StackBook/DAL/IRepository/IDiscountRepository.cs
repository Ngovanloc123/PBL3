using System.Linq.Expressions;
using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface IDiscountRepository
    {
        Task<Discount?> GetDiscountByIdAsync(Guid discountId);
    }
}
