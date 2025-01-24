using System.Net;
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
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Throw HttpRequestException with response details
                throw new HttpRequestException(
                    $"HTTP Error {response.StatusCode}: {responseContent}",
                    null,
                    response.StatusCode);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}