
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;

namespace StackBook.DAL.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Payment> GetByIdAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            return payment ?? throw new Exception("Payment not found");
        }
        public async Task<List<Payment>> GetAllAsync()
        {
            var payments = await _context.Payments.ToListAsync();
            return payments;
        }
        public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId)
        {
            var payments = await _context.Payments.Where(p => p.OrderId == orderId).ToListAsync();
            return payments;
        }
        public async Task<List<Payment>> GetByStatusAsync(string status)
        {
            var payments = await _context.Payments.Where(p => p.PaymentStatus == status).ToListAsync();
            return payments;
        }
        public async Task<List<Payment>> GetByMethodAsync(string method)
        {
            var payments = await _context.Payments.Where(p => p.PaymentMethod == method).ToListAsync();
            return payments;
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
        public async Task UpdatePaymentStatusAsync(Guid paymentId, string status)
        {
            var payment = await GetByIdAsync(paymentId);
            payment.PaymentStatus = status;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePaymentAsync(Guid paymentId)
        {
            var payment = await GetByIdAsync(paymentId);
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> PaymentExistsAsync(Guid id)
        {
            return await _context.Payments.AnyAsync(p => p.PaymentId == id);
        }
    }
}