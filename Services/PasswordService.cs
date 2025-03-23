using BCrypt.Net;

namespace TaskManagement.Services
{
    public class PasswordService
    {
        private const int WorkFactor = 12; 

        // متد هش کردن رمز عبور
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        // متد بررسی رمز عبور
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
