using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreBytes.API.Models;
using StoreBytes.DataAccess.Data;
using StoreBytes.Common.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using StoreBytes.API.Utilities;
using StoreBytes.DataAccess.Models;
using StoreBytes.API.Security;

namespace StoreBytes.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDatabaseData _db;
        private readonly IConfiguration _config;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IDatabaseData db, IConfiguration config, JwtHelper jwtHelper)
        {
            _db = db;
            _config = config;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public IActionResult GenerateToken([FromBody] TokenRequestModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ApiKey))
                {
                    return BadRequest(new { error = "API key is required." });
                }
                // Validate API key
                var userToken = _db.GetUserTokenByApiKey(request.ApiKey);

                if (userToken == null)
                {
                    return Unauthorized(new { error = "Invalid or inactive API key."});
                }

                var token = _jwtHelper.GenerateJwtToken(userToken.UserId, 15, true);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message});
            }
        }

        // POST: /auth/create-api-key
        [HttpPost("create-api-key")]
        [Authorize]
        public IActionResult CreateApiKey()
        {
            try
            {
                var tokenType = User.FindFirst(CustomClaimTypes.TokenType)?.Value;
                if (string.IsNullOrWhiteSpace(tokenType) || tokenType != TokenTypeValues.User)
                {
                    return Unauthorized(new { error = "Invalid token type. API keys cannot be used for this operation." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { error = "Invalid user ID." });
                }

                string newApiKey = _db.SaveApiKey(userId);
                return Ok(new { apiKey = newApiKey });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message});
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterUserRequestModel request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { error = "Email and password are required." });
                }

                // Validate email format
                if (!ValidationHelper.IsValidEmail(request.Email))
                {
                    return BadRequest(new { error = "Invalid email format." });
                }

                // Validate password strength
                if (!ValidationHelper.IsStrongPassword(request.Password))
                {
                    return BadRequest(new { error = "Password must be at least 8 characters." });
                }

                // Check if the email is already registered
                if (_db.GetUserByEmail(request.Email) != null)
                {
                    return Conflict(new { error = "Email is already registered." });
                }

                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, 12);

                // Save the user to the database
                _db.AddUserWithPassword(request.Email, hashedPassword);

                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message});
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestModel request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { error = "Email and password are required." });
                }

                // Retrieve user from the database
                var user = _db.GetUserByEmail(request.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new { error = "Invalid email or password." });
                }

                var token = _jwtHelper.GenerateJwtToken(user.Id, 60, false);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message});
            }
        }


    }
}
