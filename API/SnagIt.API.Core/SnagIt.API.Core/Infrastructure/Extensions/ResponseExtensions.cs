using Microsoft.AspNetCore.Mvc;
using SnagIt.API.Core.Application.Features.Shared.Models;
using System.Net;


namespace SnagIt.API.Core.Infrastructure.Extensions
{
    public static class ResponseExtensions
    {
        public static IActionResult GenerateIActionResultForResponse(this Response result)
        {
            if (result.Error is null)
            {
                return new OkObjectResult(result);
            }

            return (result.Error?.Status) switch
            {
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(result),
                (int)HttpStatusCode.Forbidden => new ObjectResult(result)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                },
                _ => throw new InvalidOperationException()
            };
        }
    }
}
