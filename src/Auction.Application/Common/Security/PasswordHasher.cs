using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace Auction.Application.Common.Security
{
    public static class PasswordHasher
    {
        public static (string, string) HashPassword(string password, byte[]? salt = null)
        {
            salt = salt ?? RandomNumberGenerator.GetBytes(128 / 8);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return (hashed, Convert.ToBase64String(salt));
        }

        public static bool IsPasswordCorrect(string inputPassword, string userPassword, string salt)
        {
            var (hashedPass, generatedSalt) = HashPassword(inputPassword, Convert.FromBase64String(salt));
            return hashedPass == userPassword;
        }
    }
}
