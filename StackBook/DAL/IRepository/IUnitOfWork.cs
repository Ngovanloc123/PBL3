
namespace StackBook.DAL.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IAuthorRepository Author { get; }
        IBookRepository Book { get; }
        IUserRepository User { get; }
        ICartRepository Cart { get; }
        IShippingAddressRepository ShippingAddress { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderRepository Order { get; }
        IOrderHistoryRepository OrderHistory { get; }
        IDiscountRepository Discount { get; }
        IPaymentRepository Payment { get; }

        Task<IDisposable> BeginTransactionAsync();
        Task SaveAsync();
    }
}