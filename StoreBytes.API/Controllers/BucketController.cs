﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytes.API.Models;
using StoreBytes.DataAccess.Data;
using StoreBytes.Common.Utilities;

namespace StoreBytes.API.Controllers
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

                // Extract user ID from the token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { error = "Invalid user ID." });
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
                // Extract user ID from the token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { error = "Invalid user ID." });
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
    }
}
