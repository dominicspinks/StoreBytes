using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytes.API.Models;
using StoreBytes.API.Security;
using StoreBytes.API.Utilities;
using StoreBytes.DataAccess.Data;

namespace StoreBytes.API.Controllers
{
    [Route("api/keys")]
    [ApiController]
    [Authorize]
    public class KeysController : ControllerBase
    {
        private readonly IDatabaseData _db;

        public KeysController(IDatabaseData db)
        {
            _db = db;
        }

        // POST: /api/keys/create
        [HttpPost("create")]
        public IActionResult CreateApiKey([FromBody] CreateApiKeyRequestModel request)
        {
            try
            {
                // Validate token type
                var tokenType = User.FindFirst(CustomClaimTypes.TokenType)?.Value;
                if (string.IsNullOrWhiteSpace(tokenType) || tokenType != TokenTypeValues.User)
                {
                    return Unauthorized(new { error = "Invalid token type. API keys cannot be used for this operation." });
                }

                // Validate user
                var userValidationResponse = UserValidationHelper.ValidateUser(User, out int userId);
                if (userValidationResponse != null)
                {
                    return userValidationResponse;
                }

                // Save API key
                string newApiKey = _db.SaveApiKey(userId, request?.Description);
                return Ok(new { apiKey = newApiKey });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: /api/keys/list
        [HttpGet("list")]
        public IActionResult GetApiKeys()
        {
            try
            {
                // Validate user
                var userValidationResponse = UserValidationHelper.ValidateUser(User, out int userId);
                if (userValidationResponse != null)
                {
                    return userValidationResponse;
                }

                // Retrieve API keys
                var apiKeys = _db.GetApiKeysByUserId(userId);
                return Ok(apiKeys);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
