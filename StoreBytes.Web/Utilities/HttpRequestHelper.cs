using System.Net.Http;
using System.Text.Json;

namespace StoreBytes.Web.Utilities
{
    public static class HttpRequestHelper
    {
        public static void AttachAuthToken(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            var token = httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP Error {response.StatusCode}: {error}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}