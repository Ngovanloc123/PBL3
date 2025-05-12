using StackBook.Models;
namespace StackBook.VMs
{
    public class BookInCartVM
    {
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }
        public int Quantity { get; set; }
    }
}
