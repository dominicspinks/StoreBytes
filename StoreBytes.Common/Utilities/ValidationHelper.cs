using System.Text.RegularExpressions;

namespace StoreBytes.Common.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidBucketName(string bucketName)
        {
            return Regex.IsMatch(bucketName, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        }

        public static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public static bool IsStrongPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
        }
    }
}
