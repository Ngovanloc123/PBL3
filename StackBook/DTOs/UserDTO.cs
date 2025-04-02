namespace StackBook.DTOs
{
    public class RegisterDto
    {
        public string ?Username { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }
    }
    public class LoginDto
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
}