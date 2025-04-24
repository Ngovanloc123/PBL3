namespace StackBook.Models
{
    public class CartDetail
    {
        public Guid CartId { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedCart { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Book? Book { get; set; }
    }
}
