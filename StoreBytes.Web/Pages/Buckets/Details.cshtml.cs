using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreBytes.Web.Services;
using StoreBytes.Web.Models;
using StoreBytes.Common.Utilities;

namespace StoreBytes.Web.Pages.Buckets
{
    public class DetailsModel : PageModel
    {
        private readonly BucketService _bucketService;
        private readonly FileService _fileService;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(BucketService bucketService, FileService fileService, ILogger<DetailsModel> logger)
        {
            _bucketService = bucketService;
            _fileService = fileService;
            _logger = logger;
        }

        public BucketModel Bucket { get; set; }
        public List<FileModel> Files { get; set; }

        public async Task<IActionResult> OnGetAsync(string hash)
        {

            try
            {
                Bucket = await _bucketService.GetBucketDetailsAsync(hash);
                Files = await _fileService.GetFilesByBucketHashAsync(hash);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while loading the bucket details. Please try again.";
                _logger.LogError(ex, "Failed to load bucket details: {BucketHash}.", hash);
            }

            if (Bucket == null)
            {
                ViewData["ErrorMessage"] = "Bucket not found.";
                return RedirectToPage("/Buckets/List");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEditBucketAsync(string hash, string bucketName)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                ViewData["ErrorMessage"] = "Bucket name is required.";
                return await OnGetAsync(hash);
            }

            if (!ValidationHelper.IsValidBucketName(bucketName))
            {
                ViewData["ErrorMessage"] = "Bucket name must start with a letter and can only contain letters, numbers, and underscores.";
                return await OnGetAsync(hash);
            }

            UpdateBucketModel bucketDetails = new()
            {
                BucketName = bucketName
            };

            try
            {
                await _bucketService.EditBucketDetailsAsync(hash, bucketDetails);
                ViewData["SuccessMessage"] = "Bucket name updated successfully.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while updating the bucket. Please try again.";
                _logger.LogError(ex, "Failed to update bucket: {BucketHash}.", hash);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteBucketAsync(string hash)
        {
            try
            {
                await _bucketService.DeleteBucketAsync(hash);
                TempData["SuccessMessage"] = "Bucket deleted successfully.";
                return RedirectToPage("/Buckets/List");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while deleting the bucket. Please try again.";
                _logger.LogError(ex, "Failed to delete bucket: {BucketHash}.", hash);
                return await OnGetAsync(hash);
            }

        }

        public async Task<IActionResult> OnPostToggleBucketStatusAsync(string hash, string isActive)
        {
            try
            {
                bool activeStatus = bool.Parse(isActive);

                if (activeStatus)
                {
                    await _bucketService.DisableBucketAsync(hash);
                    ViewData["SuccessMessage"] = "Bucket deactivated successfully.";
                }
                else
                {
                    await _bucketService.EnableBucketAsync(hash);
                    ViewData["SuccessMessage"] = "Bucket activated successfully.";
                }

                return await OnGetAsync(hash);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while changing the bucket status. Please try again.";
                _logger.LogError(ex, "Failed to toggle bucket status: {BucketHash}.", hash);
                return await OnGetAsync(hash);
            }
        }
    }
}
