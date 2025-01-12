using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreBytesLibrary.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreBytesAPI.Controllers
{
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

                if (userToken == null || !userToken.IsActive)
                {
                    return Unauthorized("Invalid or inactive API key.");
                }

                // Generate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, userToken.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(15),
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
    }
}
