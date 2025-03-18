using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<ShippingAddress>? ShippingAddresses { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [StringLength(30, ErrorMessage = "Password cannot exceed 30 characters.")]
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
        [Required]
        public bool Role { get; set; }
        public DateTime CreatedUser { get; set; }
    }
}
