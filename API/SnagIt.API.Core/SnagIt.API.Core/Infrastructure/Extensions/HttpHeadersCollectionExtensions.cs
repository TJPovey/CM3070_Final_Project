﻿using Microsoft.Azure.Functions.Worker.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SnagIt.API.Core.Infrastructure.Extensions
{
    public static class HttpHeadersCollectionExtensions
    {
        public static string GetAuthorizationHeaderValue(this HttpHeadersCollection headersCollection)
        {
            var authToken = string.Empty;

            if (headersCollection.TryGetValues("Authorization", out var authHeaderValue))
            {
                authToken = authHeaderValue.FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
            }

            return authToken;
        }

        public static ClaimsPrincipal GetClaimsPrincipal(this HttpHeadersCollection headersCollection)
        {
            var authToken = string.Empty;

            if (headersCollection.TryGetValues("Authorization", out var authHeaderValue))
            {
                authToken = authHeaderValue.FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
            }

            var handler = new JwtSecurityTokenHandler();

            if (authToken == string.Empty || !handler.CanReadToken(authToken))
            {
                return new ClaimsPrincipal();
            }

            var token = handler.ReadJwtToken(authToken);
            var identity = new ClaimsPrincipal(new ClaimsIdentity(token.Claims, "jwt"));

            return identity;
        }
    }

}
