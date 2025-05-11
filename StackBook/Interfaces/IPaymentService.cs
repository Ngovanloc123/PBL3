using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> GetPaymentByIdAsync(Guid id);
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<List<Payment>> GetPaymentsByOrderIdAsync(Guid orderId);
        Task<List<Payment>> GetPaymentsByStatusAsync(string status);
        Task<List<Payment>> GetPaymentsByMethodAsync(string method);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task UpdatePaymentStatusAsync(Guid paymentId, string status);
        Task DeletePaymentAsync(Guid paymentId);
        Task<bool> PaymentExistsAsync(Guid id);
    }
}