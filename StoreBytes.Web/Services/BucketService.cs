using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using StoreBytes.Web.Models;
using StoreBytes.Web.Utilities;

namespace StoreBytes.Web.Services
{
    public class BucketService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BucketService> _logger;

        public BucketService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<BucketService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StoreBytesAPI");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<FullBucketModel>> GetBucketsAsync()
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);
            try
            {
                _logger.LogInformation("Fetching buckets for the current user.");
                var response = await _httpClient.GetAsync("/api/buckets/list");

                var buckets = await HttpRequestHelper.HandleResponseAsync<List<FullBucketModel>>(response);
                _logger.LogInformation("Successfully retrieved {Count} buckets.", buckets.Count);

                return buckets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching buckets.");
                throw;
            }
        }

        public async Task AddBucketAsync(string bucketName)
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);

            var payload = JsonSerializer.Serialize(new { BucketName = bucketName });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            try
            {
                _logger.LogInformation("Adding a new bucket with name: {BucketName}.", bucketName);
                var response = await _httpClient.PostAsync("/api/buckets/create", content);

                await HttpRequestHelper.HandleResponseAsync<object>(response);

                _logger.LogInformation("Successfully added a new bucket: {BucketName}.", bucketName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add a new bucket: {BucketName}.", bucketName);
                throw;
            }
        }
    }
}
