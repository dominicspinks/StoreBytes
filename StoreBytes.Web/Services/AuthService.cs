using StoreBytes.Web.Models;
using System.Text;
using System.Text.Json;

namespace StoreBytes.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
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

            var response = await _httpClient.PostAsync("/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                return true; // Sign up successful
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Sign Up Failed: {error}");
        }

        // Log in an existing user
        public async Task<bool> LoginAsync(string email, string password)
        {
            var payload = new
            {
                email = email,
                password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<LoginResponse>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!string.IsNullOrEmpty(json?.Token))
                {
                    // Store the token in an HttpOnly cookie
                    var context = _httpContextAccessor.HttpContext;
                    context.Response.Cookies.Append("AuthToken", json.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Ensure HTTPS in production
                        Expires = DateTime.UtcNow.AddHours(1)
                    });

                    return true;
                }

                throw new Exception("Invalid token received from the API.");
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login Failed: {error}");
        }

        // Log out the current user
        public void Logout()
        {
            var context = _httpContextAccessor.HttpContext;
            context.Response.Cookies.Delete("AuthToken");
        }
    }
}
