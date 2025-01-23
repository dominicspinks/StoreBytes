using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytes.DataAccess.Data;
using StoreBytes.Service.Files;
using StoreBytes.Common.Utilities;
using StoreBytes.Common.Configuration;
using StoreBytes.API.Utilities;

[Route("api/files")]
[ApiController]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IDatabaseData _db;
    private readonly IConfiguration _config;
    private readonly FileStorageService _files;

    public FileController(IDatabaseData db, IConfiguration config, FileStorageService files)
    {
        _db = db;
        _config = config;
        _files = files;
    }

    [HttpPost("upload")]
    public IActionResult UploadFile(IFormFile file, [FromForm] string bucketName)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "File is required." });
        }

        // Validate the user exists
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { error = "Invalid user ID." });
        }

        // Retrieve the bucket
        var bucket = _db.GetBucketByName(bucketName, userId);
        if (bucket == null)
        {
            return NotFound(new { error = "Bucket not found." });
        }

        // Generate hashed file name
        string fileExtension = Path.GetExtension(file.FileName);
        string secret = _config[ConfigurationKeys.Shared.HashSecret] ?? "";
        string fileHash = $"{SecurityHelper.HashBase64Url($"{bucket.Id}+{file.FileName}", secret, 10)}{fileExtension}";

        // Save the file using the file storage service
        string filePath;
        using (var stream = file.OpenReadStream())
        {
            filePath = _files.SaveFile(bucket.BucketHash, fileHash, stream);
        }

        // Save metadata to the database
        _db.AddFileMetadata(bucket.Id, file.FileName, fileHash, filePath, file.Length, file.ContentType);

        return Ok(new { url = $"{filePath}" });
    }

    [HttpGet("{bucketHash}/{fileHash}")]
    [AllowAnonymous]
    public IActionResult GetFile(string bucketHash, string fileHash)
    {
        try
        {
            // Retrieve file metadata from the database
            var fileMetadata = _db.GetFileMetadata(bucketHash, fileHash);
            if (fileMetadata == null)
            {
                return NotFound(new { error = "File not found." });
            }

            // Retrieve the file from the file system
            var fileStream = _files.GetFileStream(bucketHash, fileHash);
            if (fileStream == null)
            {
                return NotFound(new { error = "File not found on disk." });
            }

            // Return the file with appropriate content type
            return File(fileStream, fileMetadata.ContentType, fileMetadata.FileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message});
        }
    }

    [HttpGet("list/{hash}")]
    public IActionResult GetFilesByBucketHash(string hash)
    {
        try
        {
            var bucket = _db.GetBucketByHash(hash);
            if (bucket == null)
            {
                return NotFound(new { error = "Bucket not found." });
            }

            // Validate ownership
            var validationResponse = UserValidationHelper.ValidateOwnership(bucket, User);
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var files = _db.GetFilesByBucketHash(hash);

            return Ok(files);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
