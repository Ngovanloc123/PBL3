using System.ComponentModel.DataAnnotations;
namespace StackBook.DTOs
{
    public class RegisterDto
    {
        public string ?Username { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }
    }
    public class SignInDto
    {
        public string ?Email { get; set; }
        public string ?Password { get; set; }
    }
    public class UpdateDto
    {
        public Guid UserId { get; set; }
        public string ?Username { get; set; }
        public string ?Email { get; set; }
    }

    public class UpdatePasswordDto
    {
        public Guid UserId { get; set; }
        public string ?Password { get; set; }
    }
    
    public class ForgotPasswordDto
    {
        public string ?Email { get; set; }
    }
    public class ResetPasswordDto
    {
        public string ?Token { get; set; } = string.Empty;
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string ?NewPassword { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string ?ConfirmPassword { get; set; }
    }
}