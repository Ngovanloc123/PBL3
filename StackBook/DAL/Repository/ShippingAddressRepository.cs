using StackBook.Models;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace StackBook.DAL
{
    public class ShippingAddressRepository : IShippingAddressRepository
    {
        private readonly ApplicationDbContext _db;

        public ShippingAddressRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ShippingAddress?> GetByIdAsync(Guid shippingAddressId)
        {
            return await _db.ShippingAddresses
                .FirstOrDefaultAsync(sa => sa.ShippingAddressId == shippingAddressId);
        }

        public async Task AddAsync(ShippingAddress shippingAddress)
        {
            await _db.ShippingAddresses.AddAsync(shippingAddress);
        }

        public async Task UpdateAsync(ShippingAddress shippingAddress)
        {
            _db.ShippingAddresses.Update(shippingAddress);
        }

        public async Task DeleteAsync(ShippingAddress shippingAddress)
        {
            _db.ShippingAddresses.Remove(shippingAddress);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}