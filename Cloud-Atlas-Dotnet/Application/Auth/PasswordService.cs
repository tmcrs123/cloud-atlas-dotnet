using Microsoft.AspNetCore.Identity;

namespace Cloud_Atlas_Dotnet.Application.Auth
{
    public interface IPasswordService
    {
        string HashPassword(string plainTextPassword);
        bool VerifyHashedPassword(string hashed, string plaintext);
    }

    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<string> _passwordHasher;

        public PasswordService(IPasswordHasher<string> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(string plainTextPassword)
        {
            return _passwordHasher.HashPassword(null, plainTextPassword);
        }
        
        public bool VerifyHashedPassword(string hashed, string plaintext)
        {
            if (string.IsNullOrEmpty(hashed) || string.IsNullOrEmpty(plaintext)) return false;

            return _passwordHasher.VerifyHashedPassword(null, hashed, plaintext) == PasswordVerificationResult.Success;
        }
    }
}
