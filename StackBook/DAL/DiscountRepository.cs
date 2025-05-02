using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StackBook.Models;
using StackBook.Data;
using StackBook.DAL.IRepository;

namespace StackBook.DAL
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _db;

        public DiscountRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Discount?> GetDiscountByIdAsync(Guid discountId)
        {
            return await _db.Discounts
                .FirstOrDefaultAsync(d => d.DiscountId == discountId);
        }
    }

}
