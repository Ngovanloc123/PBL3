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
        public string? Phone { get; set; }

        [Required]
        public string? country { get; set; }

        [Required]
        public string? city { get; set; }

        [Required]
        public string? ward { get; set; }

        [Required]
        public string? Address { get; set; }
    }
}
