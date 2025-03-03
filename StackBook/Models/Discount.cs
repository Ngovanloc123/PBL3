using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }
        public virtual Order Order { get; set; }

        [Required]
        public string DiscountName { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double price { get; set; }

        [Required]
        public DateTime CreatedDiscount { get; set; }
    }
}
