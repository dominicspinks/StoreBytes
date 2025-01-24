using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreBytes.Web.Services;
using StoreBytes.Web.Models;
using Microsoft.Extensions.Logging;

namespace StoreBytes.Web.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ApiKeyService _apiKeyService;
        private readonly UserService _userService;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(ApiKeyService apiKeyService, UserService userService, ILogger<ProfileModel> logger)
        {
            _apiKeyService = apiKeyService;
            _userService = userService;
            _logger = logger;
        }

        public UserModel User { get; set; }
        public List<ApiKeyModel> ApiKeys { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                User = await _userService.GetUserDetailsAsync();
                ApiKeys = await _apiKeyService.GetApiKeysAsync();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while loading the profile. Please try again.";
                _logger.LogError(ex, "Failed to load profile page.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddApiKeyAsync(string? description)
        {
            try
            {
                var newApiKey = await _apiKeyService.CreateApiKeyAsync(description);
                ViewData["NewApiKey"] = newApiKey;
                ViewData["SuccessMessage"] = "API key created successfully.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while creating the API key. Please try again.";
                _logger.LogError(ex, "Failed to create API key.");
            }

            return await OnGetAsync();
        }
    }
}
