using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Discount
    {
        [Key]
        public Guid DiscountId { get; set; } =Guid.NewGuid();
        public virtual Order? Order { get; set; }

        [Required]
        [StringLength(100)]
        public string? DiscountName { get; set; }

        [Required]
        [StringLength(100)]
        public string? DiscountCode { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedDiscount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
