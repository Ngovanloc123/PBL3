using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using StackBook.DAL.IRepository;
using StackBook.Data;

namespace StackBook.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IAuthorRepository Author { get; private set; }
        public IBookRepository Book { get; private set; }
        public IUserRepository User { get; private set; }
        public ICartRepository Cart { get; private set; }
        public IShippingAddressRepository ShippingAddress { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderHistoryRepository OrderHistory { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IDiscountRepository Discount { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public IReviewRepository Review { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Author = new AuthorRepository(_db);
            Book = new BookRepository(_db);
            User = new UserRepository(_db);
            Cart = new CartRepository(_db);
            ShippingAddress = new ShippingAddressRepository(_db);
            Order = new OrderRepository(_db);
            OrderHistory = new OrderHistoryRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Discount = new DiscountRepository(_db);
            Payment = new PaymentRepository(_db);
            Review = new ReviewRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<IDisposable> BeginTransactionAsync()
        {
            // Example implementation for a database transaction (e.g., using SqlConnection)
            var connection = new SqlConnection("YourConnectionString");
            await connection.OpenAsync();
            var transaction = await connection.BeginTransactionAsync();
            return transaction;

            // Alternatively, if you're using Entity Framework Core:
            // var context = new YourDbContext();
            // var transaction = await context.Database.BeginTransactionAsync();
            // return transaction;
        }
    }
}