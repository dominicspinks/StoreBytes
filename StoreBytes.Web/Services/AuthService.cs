using StoreBytes.Web.Models;
using StoreBytes.Web.Utilities;
using System.Text;
using System.Text.Json;

namespace StoreBytes.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StoreBytesAPI");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Sign up a new user
        public async Task<bool> SignUpAsync(string email, string password)
        {
            var payload = new
            {
                email = email,
                password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                _logger.LogInformation("Attempting to sign up user with email: {Email}.", email);
                var response = await _httpClient.PostAsync("/api/auth/register", content);

                await HttpRequestHelper.HandleResponseAsync<object>(response);

                _logger.LogInformation("User {Email} successfully signed upin.", email);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing up user.");
                throw;
            }
        }

        // Log in an existing user
        public async Task<bool> LoginAsync(string email, string password)
        {
            var payload = new { email, password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");


            try
            {
                _logger.LogInformation("Attempting to log in user with email: {Email}.", email);
                var response = await _httpClient.PostAsync("/api/auth/login", content);

                var loginResponse = await HttpRequestHelper.HandleResponseAsync<LoginResponseModel>(response);

                if (string.IsNullOrEmpty(loginResponse?.Token))
                {
                    throw new Exception("Invalid token received from the API.");
                }

                // Store the token in an HttpOnly cookie
                var context = _httpContextAccessor.HttpContext;
                context.Response.Cookies.Append("AuthToken", loginResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                _logger.LogInformation("User {Email} successfully logged in.", email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user {Email}.", email);
                throw;
            }


        }

        // Log out the current user
        public void Logout()
        {
            try
            {
                _logger.LogInformation("User logging out.");
                var context = _httpContextAccessor.HttpContext;
                context.Response.Cookies.Delete("AuthToken");
                _logger.LogInformation("User logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout.");
                throw;
            }
        }
    }
}
