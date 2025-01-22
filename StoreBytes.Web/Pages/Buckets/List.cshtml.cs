using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreBytes.Common.Utilities;
using StoreBytes.Web.Models;
using StoreBytes.Web.Services;

namespace StoreBytes.Web.Pages.Buckets
{
    public class ListModel : PageModel
    {
        private readonly BucketService _bucketService;

        public ListModel(BucketService bucketService)
        {
            _bucketService = bucketService;
        }

        public List<FullBucketModel> Buckets { get; set; } = new();

        [BindProperty]
        public string BucketName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Buckets = await _bucketService.GetBucketsAsync();
            Console.WriteLine("ShowAddBucketModal: " + ViewData["ShowAddBucketModal"]);
            return Page();
        }

        public async Task<IActionResult> OnPostAddBucketAsync()
        {
            // Validate the bucket name
            if (string.IsNullOrWhiteSpace(BucketName))
            {
                TempData["ErrorMessage"] = "Bucket name is required.";
                return await OnGetAsync(); // Reload the list page with the error
            }

            if (!ValidationHelper.IsValidBucketName(BucketName))
            {
                TempData["ErrorMessage"] = "Bucket name must start with a letter and can only contain letters, numbers, and underscores.";
                return await OnGetAsync(); // Reload the list page with the error
            }

            try
            {
                await _bucketService.AddBucketAsync(BucketName);
                TempData["SuccessMessage"] = "Bucket created successfully!";
                return RedirectToPage(); // Refresh the page to show the updated list
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the bucket.";
                // Log the exception for debugging
                Console.WriteLine(ex);
                return await OnGetAsync(); // Reload the list page with the error
            }
        }
    }
}
