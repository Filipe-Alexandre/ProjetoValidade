using System.Security.Cryptography;
using System.Text;

namespace ValiKop.Api.Security
{
    public static class PasswordGenerator
    {
        private const string Chars =
            "ABCDEFGHJKLMNPQRSTUVWXYZ" +
            "abcdefghijkmnopqrstuvwxyz" +
            "23456789" +
            "!@#$%.";

        public static string Generate(int length = 10)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            var sb = new StringBuilder(length);

            foreach (var b in bytes)
            {
                sb.Append(Chars[b % Chars.Length]);
            }

            return sb.ToString();
        }
    }
}
