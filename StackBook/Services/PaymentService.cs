using StackBook.DAL;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;

namespace StackBook.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            return await _paymentRepository.GetByIdAsync(id);
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllAsync();
        }

        public async Task<List<Payment>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            return await _paymentRepository.GetByOrderIdAsync(orderId);
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty", nameof(status));
            }

            return await _paymentRepository.GetByStatusAsync(status);
        }

        public async Task<List<Payment>> GetPaymentsByMethodAsync(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("Method cannot be null or empty", nameof(method));
            }

            return await _paymentRepository.GetByMethodAsync(method);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            // Set creation timestamp if not already set
            if (payment.CreatedPayment == default)
            {
                payment.CreatedPayment = DateTime.UtcNow;
            }

            return await _paymentRepository.CreatePaymentAsync(payment);
        }

        public async Task UpdatePaymentStatusAsync(Guid paymentId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty", nameof(status));
            }

            await _paymentRepository.UpdatePaymentStatusAsync(paymentId, status);
        }

        public async Task DeletePaymentAsync(Guid paymentId)
        {
            await _paymentRepository.DeletePaymentAsync(paymentId);
        }

        public async Task<bool> PaymentExistsAsync(Guid id)
        {
            return await _paymentRepository.PaymentExistsAsync(id);
        }
    }
}