using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytesLibrary.Data;
using StoreBytesLibrary.Services;
using StoreBytesLibrary.Utilities;

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
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int bucketId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        // Validate the user exists
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        // Retrieve the bucket
        var bucket = _db.GetBucketById(bucketId, userId);
        if (bucket == null)
        {
            return NotFound("Bucket not found.");
        }

        // Generate hashed file name
        string fileExtension = Path.GetExtension(file.FileName);
        string secret = ConfigurationHelper.GetHashingSecret(_config);
        string hashedName = $"{SecurityHelper.HashBase64Url($"{bucketId}+{file.FileName}", secret, 10)}{fileExtension}";

        // Save the file using the file storage service
        string filePath;
        using (var stream = file.OpenReadStream())
        {
            filePath = _files.SaveFile(bucket.HashedName, hashedName, stream);
        }

        // Save metadata to the database
        _db.AddFileMetadata(bucketId, file.FileName, hashedName, filePath, file.Length, file.ContentType);

        return Ok("File uploaded successfully.");
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
                return NotFound("File not found.");
            }

            // Retrieve the file from the file system
            var fileStream = _files.GetFileStream(bucketHash, fileHash);
            if (fileStream == null)
            {
                return NotFound("File not found on disk.");
            }

            // Return the file with appropriate content type
            return File(fileStream, fileMetadata.ContentType, fileMetadata.OriginalName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
