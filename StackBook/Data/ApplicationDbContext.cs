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
    }
}
