using StackBook.Models;
using System;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface IDiscountRepository
    {
        Task<Discount?> GetByIdAsync(Guid discountId);
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(Discount discount);
        Task SaveChangesAsync();
    }
}