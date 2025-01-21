using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StoreBytes.Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear the AuthToken cookie
            HttpContext.Response.Cookies.Delete("AuthToken");

            // Redirect to the login page
            return RedirectToPage("/Auth/Login");
        }
    }
}
