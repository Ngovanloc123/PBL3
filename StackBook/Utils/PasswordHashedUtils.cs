using BCrypt.Net;

namespace StackBook.Utils
{
    public class PasswordHashedUtils
    {
        private const int WorkFactor = 12;
        public async static Task<string>  HashPassword(string password)
        {
            if(string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required");
            }
            try
            {
                return await Task.Run(() =>  BCrypt.Net.BCrypt.HashPassword(password, WorkFactor));
            }
            catch (Exception ex)
            {
                throw new Exception("Error hashing password", ex);
            }
        }
        public async static Task<bool> VerifyPassword(string password, string hashedPassword)
        {
            if(string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required");
            }
            if(string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException("Hashed password is required");
            }
            try
            {
                return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, hashedPassword));
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying password", ex);
            }
        }
    }
}