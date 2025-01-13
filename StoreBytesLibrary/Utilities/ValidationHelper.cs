using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoreBytesLibrary.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidBucketName(string bucketName)
        {
            return Regex.IsMatch(bucketName, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        }
    }
}
