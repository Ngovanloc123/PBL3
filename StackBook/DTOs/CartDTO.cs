using StackBook.Models;
namespace StackBook.DTOs
{
    public class BookInCartDto
    {
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }
        public int Quantity { get; set; }
    }
}
