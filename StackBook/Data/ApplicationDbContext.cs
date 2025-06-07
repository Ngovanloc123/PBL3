using Microsoft.EntityFrameworkCore;
using StackBook.Models;

namespace StackBook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSets (Tables) here
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ReturnOrder> ReturnOrders { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập quan hệ 1-n giữa User và Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Thay NoAction bằng Cascade

            // Thiết lập quan hệ 1-n giữa Order và Payment
            modelBuilder.Entity<Payment>()
                .HasOne(o => o.Order)
                .WithMany(p => p.Payments)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Thay NoAction bằng Cascade

            // Thiết lập quan hệ 1-1 giữa Order và ReturnOrder
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ReturnOrder)
                .WithOne(ro => ro.Order)
                .HasForeignKey<ReturnOrder>(ro => ro.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Thay NoAction bằng Cascade

            // Thiết lập quan hệ 1-n giữa User và Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Thay NoAction bằng Cascade

            // Thiết lập quan hệ 1-n giữa Book và Review là khóa ngoại và bảo toàn dữ liệu để là noaction 
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.NoAction); // Giữ nguyên NoAction để bảo toàn dữ liệu

            // Thiết lập quan hệ 1-n giữa User và ShippingAddress
            modelBuilder.Entity<ShippingAddress>()
            .HasOne(sa => sa.User)
            .WithMany(u => u.ShippingAddresses)
            .HasForeignKey(sa => sa.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Thay NoAction bằng Cascade

            // Thiết lập quan hệ 1-n giữa ShippingAddress và Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(sa => sa.Orders)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc Cascade tùy nghiệp vụ

            // Thiết lập quan hệ 1-n giữa ShippingAddress và ReturnOrder
            modelBuilder.Entity<ReturnOrder>()
                .HasOne(ro => ro.ShippingAddress)
                .WithMany(sa => sa.ReturnOrders)
                .HasForeignKey(ro => ro.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc Cascade tùy nghiệp vụ

            // Thiết lập quan hệ 1-n giữa Order và OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ nguyên vì chi tiết đơn hàng nên xóa theo

            // Thiết lập quan hệ n-n giữa Book và Cart
            modelBuilder.Entity<CartDetail>()
                .HasKey(cb => new { cb.CartId, cb.BookId });

            modelBuilder.Entity<CartDetail>()
                .HasOne(cb => cb.Cart)
                .WithMany(cb => cb.CartDetails)
                .HasForeignKey(cb => cb.CartId)
                .OnDelete(DeleteBehavior.Cascade); // Thêm Cascade cho xóa

            modelBuilder.Entity<CartDetail>()
                .HasOne(cb => cb.Book)
                .WithMany(cb => cb.CartDetails)
                .HasForeignKey(cb => cb.BookId)
                .OnDelete(DeleteBehavior.Cascade); // Thêm Cascade cho xóa
            // Thiết lập quan hệ 1-n giữa Review và Order
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.NoAction); // Giữ nguyên NoAction để bảo toàn dữ liệu
        }
    }
}