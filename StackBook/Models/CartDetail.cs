using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }
        [Required]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        [Required]
        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double price { get; set; }

    }
}
