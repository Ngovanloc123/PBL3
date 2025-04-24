using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<ShippingAddress>? ShippingAddresses { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        public string? Password { get; set; }
        [Required]
        public bool Role { get; set; }
        public DateTime CreatedUser { get; set; } = DateTime.Now;

        public string? ResetPasswordToken { get; set; } = "";
        public DateTime? ResetTokenExpiry { get; set; } = null;

        public bool LockStatus { get; set; } = true;
        public DateTime? DateLock { get; set; } = DateTime.MinValue;
        public int AmountOfTime { get; set; } = 0;
        public bool IsEmailVerified { get; set; } = false;
        public DateTime? EmailVerifiedAt { get; set; } = DateTime.MinValue;
        [StringLength(100)]
        public string? VerificationToken { get; set; } = "";
    }
}
