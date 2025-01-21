
namespace StoreBytes.Service.Files
{
    public class FileStorageService
    {
        private readonly string _baseDirectory;

        public FileStorageService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
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

            return Path.Combine(bucketHash, hashedFileName);
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
    }
}
