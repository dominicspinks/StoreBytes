using StoreBytes.Web.Models;
using StoreBytes.Web.Utilities;

namespace StoreBytes.Web.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StoreBytesAPI");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<UserModel> GetUserDetailsAsync()
        {
            HttpRequestHelper.AttachAuthToken(_httpClient, _httpContextAccessor);

            try
            {
                _logger.LogInformation("Fetching user details.");
                var response = await _httpClient.GetAsync("/api/users/details");

                var user = await HttpRequestHelper.HandleResponseAsync<UserModel>(response);
                _logger.LogInformation("Successfully retrieved user details.");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user details.");
                throw;
            }
        }
    }
}
