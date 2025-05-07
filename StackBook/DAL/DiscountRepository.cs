using StackBook.Models;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace StackBook.DAL
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _db;

        public DiscountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Discount?> GetByIdAsync(Guid discountId)
        {
            return await _db.Discounts
                .FirstOrDefaultAsync(d => d.DiscountId == discountId);
        }

        public async Task AddAsync(Discount discount)
        {
            await _db.Discounts.AddAsync(discount);
        }

        public async Task UpdateAsync(Discount discount)
        {
            _db.Discounts.Update(discount);
        }

        public async Task DeleteAsync(Discount discount)
        {
            _db.Discounts.Remove(discount);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}