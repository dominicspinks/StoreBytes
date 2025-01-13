using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBytesLibrary.Utilities
{
    public static class ConfigurationHelper
    {
        public static string GetHashingSecret(IConfiguration config)
        {
            string? secret = config["Hashing:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("Hashing secret is not configured.");
            }

            return secret;
        }

        public static string GetFileStorageBasePath(IConfiguration config)
        {
            string? basePath = config["FileStorage:BasePath"];
            if (string.IsNullOrEmpty(basePath))
            {
                throw new Exception("File storage base path is not configured.");
            }

            return basePath;
        }

    }
}
