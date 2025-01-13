﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytesAPI.Models;
using StoreBytesLibrary.Data;
using StoreBytesLibrary.Utilities;

namespace StoreBytesAPI.Controllers
{
    [Route("api/buckets")]
    [ApiController]
    [Authorize] // Ensures that the endpoint requires a valid token
    public class BucketController : ControllerBase
    {
        private readonly IDatabaseData _db;

        public BucketController(IDatabaseData db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public IActionResult CreateBucket([FromBody] CreateBucketRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.BucketName))
                {
                    return BadRequest("Bucket name is required.");
                }

                // Validate bucket name format
                if (!ValidationHelper.IsValidBucketName(request.BucketName))
                {
                    return BadRequest("Bucket name must start with a letter and contain only alphabetic characters.");
                }

                // Extract user ID from the token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user ID.");
                }

                // Create the bucket
                _db.CreateBucket(userId, request.BucketName);

                return Ok("Bucket created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
