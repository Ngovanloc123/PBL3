using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string? AvatarUrl { get; set; } = "https://www.gravatar.com/avatar/default?s=200&d=mp";
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
        public bool Role { get; set; } // true = Admin, false = User
        
        public DateTime CreatedUser { get; set; }

        public string? GoogleId { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        public bool LockStatus { get; set; } = true;
        public DateTime? DateLock { get; set; }
        
        public int AmountOfTime { get; set; }
        
        public bool IsEmailVerified { get; set; } = false;
        public DateTime? EmailVerifiedAt { get; set; }

        [MaxLength(512)]
        public string? VerificationToken { get; set; }
        
        // Các trường liên quan đến Refresh Token
        public string? RefreshToken { get; set; } // Thêm RefreshToken để lưu trữ Refresh Token
        public DateTime? RefreshTokenExpiry { get; set; } // Thêm thời gian hết hạn của Refresh Token
    }
}
