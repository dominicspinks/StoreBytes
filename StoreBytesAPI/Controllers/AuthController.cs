using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreBytesAPI.Models;
using StoreBytesLibrary.Data;
using StoreBytesLibrary.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreBytesAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDatabaseData _db;
        private readonly IConfiguration _config;

        public AuthController(IDatabaseData db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] string apiKey)
        {
            try
            {
                // Validate API key
                var userToken = _db.GetUserTokenByApiKey(apiKey);

                if (userToken == null)
                {
                    return Unauthorized("Invalid or inactive API key.");
                }

                // Generate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["JWT_SECRET"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, userToken.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHandler.WriteToken(token) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: /auth/test
        [HttpGet("test")]
        [Authorize]
        public IActionResult TestToken()
        {
            // This endpoint will only be accessible with a valid token
            return Ok("Token is valid!");
        }

        // POST: /auth/create-api-key
        [HttpPost("create-api-key")]
        [Authorize]
        public IActionResult CreateApiKey([FromBody] int userId)
        {
            try
            {
                _db.SaveApiKey(userId);
                return Ok("API key generated successfully. Check logs for the key.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: /auth/add-user
        [HttpPost("add-user")]
        [Authorize]
        public IActionResult AddUser([FromBody] AddUserRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("Email is required.");
                }

                // Add the user to the database
                _db.AddUser(request.Email);

                return Ok("User added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
