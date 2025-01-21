using System.Security.Cryptography;
using System.Text;

namespace StoreBytes.Common.Utilities
{
    public static class SecurityHelper
    {
        public static string GenerateApiKey(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        public static string HashHex(string input, string secret = "", int length = 0)
        {
            return Hash(input, secret, false, length);
        }

        public static string HashBase64(string input, string secret = "", int length = 0)
        {
            return Hash(input, secret, true, length);
        }

        public static string HashBase64Url(string input, string secret = "", int length = 0)
        {
            return Hash(input, secret, true, length)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        private static string Hash(string input, string secret = "", bool useBase64 = false, int length = 0)
        {
            using (var sha256 = SHA256.Create())
            {
                string data = $"{input}{secret}";
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

                string hash = useBase64 ? Convert.ToBase64String(bytes) : BitConverter.ToString(bytes).Replace("-", "").ToLower();

                if (length > 0)
                {
                    return hash.Substring(0, length);
                }
                else
                {
                    return hash;
                }
            }
        }
    }
}
