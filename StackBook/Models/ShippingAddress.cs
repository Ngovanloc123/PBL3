using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class ShippingAddress
    {
        [Key]
        public Guid ShippingAddressId { get; set; } = Guid.NewGuid();
         public virtual ICollection<Order>? Orders { get; set; }
         public virtual ICollection<ReturnOrder>? ReturnOrders { get; set; }

        [Required]
        public Guid UserId { get; set; }
         public virtual User? User { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string? Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        public string? Country { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string? City { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Ward cannot exceed 100 characters.")]
        public string? Ward { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string? Address { get; set; }
    }
}
