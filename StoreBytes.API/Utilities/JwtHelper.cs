using Microsoft.IdentityModel.Tokens;
using StoreBytes.API.Security;
using StoreBytes.Common.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreBytes.API.Utilities
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(int userId, int expirationMinutes, bool isApiKeyToken = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config[ConfigurationKeys.Shared.JwtSecret]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            if (isApiKeyToken)
            {
                claims.Add(new Claim(CustomClaimTypes.TokenType, TokenTypeValues.Api));
            }
            else
            {
                claims.Add(new Claim(CustomClaimTypes.TokenType, TokenTypeValues.User));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}