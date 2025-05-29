using StackBook.Models;
namespace StackBook.DAL.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetByIdAsync(Guid id);
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetByOrderIdAsync(Guid orderId);
        Task<List<Payment>> GetByStatusAsync(string status);
        Task<List<Payment>> GetByMethodAsync(string method);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task UpdatePaymentStatusAsync(Guid paymentId, string status);
        Task DeletePaymentAsync(Guid paymentId);
        Task<bool> PaymentExistsAsync(Guid id);
    }
}