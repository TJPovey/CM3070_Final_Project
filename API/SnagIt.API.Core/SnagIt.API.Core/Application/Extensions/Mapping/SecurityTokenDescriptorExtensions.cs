using Microsoft.IdentityModel.Tokens;
using SnagIt.API.Core.Domain.Aggregates.User;
using System.IdentityModel.Tokens.Jwt;
using static SnagIt.API.Core.Application.Models.User.TokenDto;

namespace SnagIt.API.Core.Application.Extensions.Mapping
{
    public static class SecurityTokenDescriptorExtensions
    {
        public static TokenDetailItem ToTokenDetailtem(this SecurityTokenDescriptor tokenDescriptor, SnagItUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);


            return new TokenDetailItem()
            {
                Kind = nameof(TokenDetailItem),
                Id = user.Id.ToString(),
                Item = new TokenDetail
                {
                    TokenType = tokenDescriptor.TokenType,
                    ExpiresIn = tokenDescriptor.Expires,
                    AccessToken = tokenString
                }
            };
        }
    }
}
