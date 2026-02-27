using System.Security.Cryptography;
using System.Text;

namespace ValiKop.Api.Security
{
    public static class PasswordHasher
    {
        public static (string hash, string salt) Hash(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);

            string salt = Convert.ToBase64String(saltBytes);

            using var sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(password + salt)
            );

            string hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }

        public static bool Verify(string password, string hash, string salt)
        {
            using var sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(password + salt)
            );

            string computedHash = Convert.ToBase64String(hashBytes);

            return computedHash == hash;
        }
    }
}
