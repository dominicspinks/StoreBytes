using System.Text.RegularExpressions;

namespace StoreBytes.Common.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidBucketName(string bucketName)
        {
            return Regex.IsMatch(bucketName, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        }
    }
}
