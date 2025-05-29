using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.DAL.Repository
{
    public class DiscountRepository: Repository<Discount>, IDiscountRepository
    {
        private readonly ApplicationDbContext _context;
        public DiscountRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<Discount> GetByIdAsync(Guid id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            return discount ?? throw new Exception("Discount not found");
        }
        public async Task<List<Discount>> GetAllAsync()
        {
            var discounts = await _context.Discounts.ToListAsync();
            return discounts;
        }
        public async Task<List<Discount>> GetActiveDiscountsAsync(DateTime currentDate)
        {
            var discounts = await _context.Discounts.Where(d => d.StartDate <= currentDate && d.EndDate >= currentDate).ToListAsync();
            return discounts;
        }
        public async Task<List<Discount>> GetExpiredDiscountsAsync(DateTime currentDate)
        {
            var discounts = await _context.Discounts.Where(d => d.EndDate < currentDate).ToListAsync();
            return discounts;
        }
        public async Task<List<Discount>> GetUpcomingDiscountsAsync(DateTime currentDate)
        {
            var discounts = await _context.Discounts.Where(d => d.StartDate > currentDate).ToListAsync();
            return discounts;
        }
        public async Task<Discount> GetByCodeAsync(string code)
        {
            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.DiscountCode == code);
            if(discount == null) 
                throw new Exception("Discount not found");
            return discount;
        }
        public async Task AddAsync(Discount entity)
        {
            await _context.Discounts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Discount entity)
        {
            _context.Discounts.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var discount = await GetByIdAsync(id);
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Discounts.AnyAsync(d => d.DiscountId == id);
        }
    }
}
