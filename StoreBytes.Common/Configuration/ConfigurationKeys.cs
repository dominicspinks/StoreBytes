namespace StoreBytes.Common.Configuration
{
    public static class ConfigurationKeys
    {
        public static class Shared
        {
            public const string JwtSecret = "JWT_SECRET";
            public const string HashSecret = "HASH_SECRET";
        }

        // Variables specific to the web application
        public static class WebApp
        {
            public const string StoreBytesApiUrl = "STOREBYTESAPI_URL";
        }

        // Variables specific to the API
        public static class Api
        {
            // Environment Variables
            public const string DatabaseUrl = "DATABASE_URL";

            // appsettings.json
            public const string FilesBasePath = "FilesBasePath";
        }
    }
}
