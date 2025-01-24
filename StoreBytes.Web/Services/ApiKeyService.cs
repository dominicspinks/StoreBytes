using StoreBytes.Web.Utilities;
using System.Text.Json;
using System.Text;
using StoreBytes.Web.Models;

namespace StoreBytes.Web.Services
{
    public class ApiKeyService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiKeyService> _logger;

        public ApiKeyService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ApiKeyService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StoreBytesAPI");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<ApiKeyModel>> GetApiKeysAsync()
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);

            try
            {
                _logger.LogInformation("Fetching API keys for the current user.");
                var response = await _httpClient.GetAsync("/api/keys/list");

                var apiKeys = await HttpRequestHelper.HandleResponseAsync<List<ApiKeyModel>>(response);
                _logger.LogInformation("Successfully retrieved {Count} API keys.", apiKeys.Count);

                return apiKeys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching API keys.");
                throw;
            }
        }

        public async Task<string> CreateApiKeyAsync(string? description = null)
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);

            var payload = JsonSerializer.Serialize(new { Description = description });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            try
            {
                _logger.LogInformation("Creating a new API key with description: {Description}.", description);
                var response = await _httpClient.PostAsync("/api/keys/create", content);

                var result = await HttpRequestHelper.HandleResponseAsync<ApiKeyResponseModel>(response);

                _logger.LogInformation("Successfully created a new API key.");
                return result.ApiKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a new API key.");
                throw;
            }
        }
    }
}
