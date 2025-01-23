using StoreBytes.Web.Models;
using StoreBytes.Web.Utilities;

namespace StoreBytes.Web.Services
{
    public class FileService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BucketService> _logger;

        public FileService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<BucketService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StoreBytesAPI");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<FileModel>> GetFilesByBucketHashAsync(string bucketHash)
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);

            try
            {
                _logger.LogInformation("Fetching files for the bucket with hash: {BucketHash}.", bucketHash);
                var response = await _httpClient.GetAsync($"/api/files/list/{bucketHash}");

                var files = await HttpRequestHelper.HandleResponseAsync<List<FileModel>>(response);

                // Generate FullUrl for each file
                foreach (var file in files)
                {
                    file.Url = $"{_httpClient.BaseAddress}api/files/{file.FilePath}";
                }

                _logger.LogInformation("Successfully retrieved {Count} files.", files.Count);

                return files ?? new List<FileModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching files.");
                throw;
            }
        }

        public async Task DeleteFileAsync(string bucketHash, string fileHash)
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);

            try
            {
                _logger.LogInformation("Deleting file with hash: {fileHash}", fileHash);
                var response = await _httpClient.DeleteAsync($"/api/files/{bucketHash}/{fileHash}");
                await HttpRequestHelper.HandleResponseAsync<object>(response);
                _logger.LogInformation("Successfully deleted file with hash: {fileHash}", fileHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file.");
                throw;
            }
            
        }
    }
}
