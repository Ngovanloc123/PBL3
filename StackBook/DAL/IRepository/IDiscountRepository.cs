using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface IDiscountRepository : IRepository<Discount>
    {
        Task<Discount> GetByIdAsync(Guid id);
        Task<List<Discount>> GetAllAsync();
        Task<List<Discount>> GetActiveDiscountsAsync(DateTime currentDate);
        Task<List<Discount>> GetExpiredDiscountsAsync(DateTime currentDate);
        Task<List<Discount>> GetUpcomingDiscountsAsync(DateTime currentDate);
        Task<Discount> GetByCodeAsync(string code);
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
