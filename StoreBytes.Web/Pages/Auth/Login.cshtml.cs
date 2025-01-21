using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreBytes.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace StoreBytes.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        [Required]
        public string? Email { get; set; }

        [BindProperty]
        [Required]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            try
            {
                // Log in the user
                var success = await _authService.LoginAsync(Email, Password);

                if (success)
                {
                    // Redirect to the index page upon successful login
                    return RedirectToPage("/Index");
                }

                ErrorMessage = "Login failed.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }
        }
    }
}
