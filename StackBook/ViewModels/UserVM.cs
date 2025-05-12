using System.ComponentModel.DataAnnotations;

namespace StackBook.ViewModels
{
    public class UserVM
    {
        public class RegisterVM
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            public string? Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required.")]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public class SignInVM
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            public string? Password { get; set; }
        }

        public class UpdateVM
        {
            [Required(ErrorMessage = "UserId is required.")]
            public Guid UserId { get; set; }

            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }
        }

        public class UpdatePasswordVM
        {
            [Required(ErrorMessage = "UserId is required.")]
            public Guid UserId { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            public string? Password { get; set; }
        }

        public class ForgotPasswordVM
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }
        }

        public class ResetPasswordVM
        {
            [Required(ErrorMessage = "Token is required.")]
            public string? Token { get; set; } = string.Empty;

            [Required(ErrorMessage = "New password is required.")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            public string? NewPassword { get; set; }

            [Required(ErrorMessage = "Confirm password is required.")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string? ConfirmPassword { get; set; }
        }
    }
}
