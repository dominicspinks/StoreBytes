
using Microsoft.Extensions.Logging;

namespace StoreBytes.Service.Files
{
    public class FileStorageService
    {
        private readonly string _baseDirectory;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(string baseDirectory, ILogger<FileStorageService> logger)
        {
            _baseDirectory = baseDirectory;
            _logger = logger;
        }

        public string SaveFile(string bucketHash, string hashedFileName, Stream fileStream)
        {
            // Ensure bucket directory exists
            string bucketPath = Path.Combine(_baseDirectory, bucketHash);
            Directory.CreateDirectory(bucketPath);

            // Define the full file path
            string filePath = Path.Combine(bucketPath, hashedFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.CopyTo(stream);
            }

            return $"{bucketHash}/{hashedFileName}";
        }

        public Stream? GetFileStream(string bucketHash, string fileHash)
        {
            // Construct the file path
            string filePath = Path.Combine(_baseDirectory, bucketHash, fileHash);

            // Return the file stream if the file exists
            if (File.Exists(filePath))
            {
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }

            return null;
        }

        public void DeleteFile(string bucketHash, string fileHash)
        {
            // Construct the file path
            string filePath = Path.Combine(_baseDirectory, bucketHash, fileHash);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File does not exist: {filePath}", filePath);
                return;
            }

            try
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FilePath}", filePath);
                throw new IOException($"Failed to delete file at {filePath}", ex);
            }
        }
    }
}
