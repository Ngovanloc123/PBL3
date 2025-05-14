namespace StackBook.ViewModels
{
    public class BookInCartVM
    {
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
