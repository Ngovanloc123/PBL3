using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class CartDetail
    {
        [Key]
        public Guid CartDetailId { get; set; } = Guid.NewGuid();
        [Required]
        public Guid CartId { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }

        [Required]
        public Guid BookId { get; set; }
        public virtual Book? Book { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double price { get; set; }

    }
}
