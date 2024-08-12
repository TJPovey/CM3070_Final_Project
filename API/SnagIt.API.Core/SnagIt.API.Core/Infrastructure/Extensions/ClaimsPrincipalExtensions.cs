using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.FindFirst("userName")?.Value;

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new InvalidOperationException("userName claim could not be found.");
            }

            return userName;
        }


        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirst("userId")?.Value;

            if (userId is null || !Guid.TryParse(userId, out var result))
            {
                throw new InvalidOperationException("User Identifier claim, userId, could not be found.");
            }

            return result;
        }
    }
}
