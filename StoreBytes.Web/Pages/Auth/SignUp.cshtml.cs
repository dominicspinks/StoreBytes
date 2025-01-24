using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreBytes.Web.Services;

namespace StoreBytes.Web.Pages.Auth
{
    public class SignUpModel : PageModel
    {
        private readonly AuthService _authService;

        public SignUpModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            if (Password.Length < 8)
            {
                ErrorMessage = "Password must be at least 8 characters long.";
                return Page();
            }

            try
            {
                // Call AuthService to register the user
                var success = await _authService.SignUpAsync(Email, Password);

                if (success)
                {
                    // Redirect to login page after successful sign-up
                    return RedirectToPage("/Auth/Login");
                }

                ErrorMessage = "Sign up failed.";
                return Page();
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
