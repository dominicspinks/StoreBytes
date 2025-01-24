using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytes.API.Models;
using StoreBytes.API.Utilities;
using StoreBytes.DataAccess.Data;
using StoreBytes.Common.Utilities;

namespace StoreBytes.API.Controllers
{
    [Route("api/buckets")]
    [ApiController]
    [Authorize]
    public class BucketController : ControllerBase
    {
        private readonly IDatabaseData _db;

        public BucketController(IDatabaseData db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public IActionResult CreateBucket([FromBody] CreateBucketRequestModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.BucketName))
                {
                    return BadRequest(new { error = "Bucket name is required." });
                }

                // Validate bucket name format
                if (!ValidationHelper.IsValidBucketName(request.BucketName))
                {
                    return BadRequest(new { error = "Bucket name must start with a letter and contain only alphabetic characters." });
                }

                // Validate user
                var validationResponse = UserValidationHelper.ValidateUser(User, out int userId);
                if (validationResponse != null)
                {
                    return validationResponse;
                }

                // Create the bucket
                _db.CreateBucket(userId, request.BucketName);

                return Ok(new { message = "Bucket created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message});
            }
        }

        [HttpGet("list")]
        public IActionResult GetBuckets()
        {
            try
            {
                // Validate user
                var validationResponse = UserValidationHelper.ValidateUser(User, out int userId);
                if (validationResponse != null)
                {
                    return validationResponse;
                }

                // Retrieve buckets for the user
                var buckets = _db.GetBucketsByUserId(userId);

                return Ok(buckets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message});
            }
        }

        [HttpGet("details/{hash}")]
        public IActionResult GetBucketDetails(string hash)
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

                return Ok(bucket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("{hash}/enable")]
        public IActionResult EnableBucket(string hash)
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

                // Enable the bucket
                var result = _db.SetBucketActiveState(hash, true);
                if (!result)
                {
                    return BadRequest(new { error = "Bucket is already enabled." });
                }

                return Ok(new { message = "Bucket successfully enabled." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("{hash}/disable")]
        public IActionResult DisableBucket(string hash)
        {
            try
            {
                var bucket = _db.GetBucketByHash(hash);
                if (bucket == null)
                {
                    return NotFound(new { error = "Bucket not found." });
                }

                var validationResponse = UserValidationHelper.ValidateOwnership(bucket, User);
                if (validationResponse != null)
                {
                    return validationResponse;
                }

                _db.SetBucketActiveState(hash, false);
                return Ok(new { message = "Bucket successfully disabled." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpDelete("{hash}")]
        public IActionResult DeleteBucket(string hash)
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

                // Delete the bucket
                var result = _db.DeleteBucket(hash);
                if (!result)
                {
                    return BadRequest(new { error = "Failed to delete the bucket." });
                }

                return Ok(new { message = "Bucket successfully deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("{hash}/update")]
        public IActionResult UpdateBucket(string hash, [FromBody] UpdateBucketRequestModel request)
        {
            try
            {
                if (request == null || (request.BucketName == null && request.IsActive == null))
                {
                    return BadRequest(new { error = "At least one field (BucketName or IsActive) must be provided for the update." });
                }

                var bucket = _db.GetBucketByHash(hash);
                if (bucket == null)
                {
                    return NotFound(new { error = "Bucket not found." });
                }

                var validationResponse = UserValidationHelper.ValidateOwnership(bucket, User);
                if (validationResponse != null)
                {
                    return validationResponse;
                }

                bool success = _db.UpdateBucketDetails(
                    bucketHash: hash,
                    bucketName: request.BucketName ?? bucket.BucketName,
                    isActive: request.IsActive ?? bucket.IsActive
                );

                if (!success)
                {
                    return BadRequest(new { error = "Failed to update the bucket." });
                }
                return Ok(new { message = $"Bucket {hash} successfully updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
