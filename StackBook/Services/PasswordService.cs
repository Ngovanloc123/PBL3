using BCrypt.Net;

namespace StackBook.Services
{
    public class PasswordService : IPasswordService
    {
        public Task<string> HashPassword(string password)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
        }

        public Task<bool> VerifyPassword(string password, string hashedPassword)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
        }
    }
}
