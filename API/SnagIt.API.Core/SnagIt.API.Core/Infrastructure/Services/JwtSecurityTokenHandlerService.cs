using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SnagIt.API.Core.Infrastructure.Services
{
    public interface IJwtSecurityTokenHandlerService
    {
        SecurityTokenDescriptor GenerateToken(Guid userId, string username, string email);
        ClaimsPrincipal ValidateJwtToken(string token);
    }

    public class JwtSecurityTokenHandlerService : IJwtSecurityTokenHandlerService
    {

        public SecurityTokenDescriptor GenerateToken(Guid userId, string username, string email)
        {
            var signingKey = Environment.GetEnvironmentVariable("SigningKey");
            var key = Encoding.ASCII.GetBytes(signingKey);

            var claims = new Dictionary<string, object>
                    {
                        { "userId", userId.ToString() },
                        { "userName", username },
                        { "email", email }
                    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Claims = claims,
                Audience = "SnagItClient",
                Issuer = "SnagItApp",
                TokenType = "Bearer"
            };

            return tokenDescriptor;
        }

        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            var signingKey = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SigningKey")!);

            // Create a token handler and validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = "SnagItApp",
                ValidAudience = "SnagItClient",
                IssuerSigningKey = new SymmetricSecurityKey(signingKey)
            }, out SecurityToken validatedToken);

            // Return the claims principal
            return claimsPrincipal;
        }
    }
}
