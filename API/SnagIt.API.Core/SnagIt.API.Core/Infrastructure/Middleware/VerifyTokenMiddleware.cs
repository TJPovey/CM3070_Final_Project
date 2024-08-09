using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using SnagIt.API.Core.Infrastructure.Extensions;
using SnagIt.API.Core.Infrastructure.Services;
using System.Net;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Features.Shared.Models;

namespace SnagIt.API.Core.Infrastructure.Middleware
{
    public class VerifyTokenMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IJwtSecurityTokenHandlerService _jwtSecurityTokenHandlerService;

        public VerifyTokenMiddleware(IJwtSecurityTokenHandlerService jwtSecurityTokenHandlerService)
        {
            _jwtSecurityTokenHandlerService = jwtSecurityTokenHandlerService ?? throw new ArgumentNullException(nameof(jwtSecurityTokenHandlerService));
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var requestData = await context.GetHttpRequestDataAsync();
            var jwtToken = requestData.Headers.GetAuthorizationHeaderValue();

            try
            {
                _jwtSecurityTokenHandlerService.ValidateJwtToken(jwtToken);
            }
            catch
            {
                var forbiddenResponse = GenerateForbiddenResponse();
                var actionResponse = forbiddenResponse.GenerateIActionResultForResponse();
                context.GetInvocationResult().Value = actionResponse;

                return;
            }

            await next(context);
        }

        private static ForbiddenDto GenerateForbiddenResponse()
        {
            var error = new ResponseError
            {
                Type = "",
                Title = $"Forbidden",
                Status = (int)HttpStatusCode.Forbidden,
                Detail = "Only authenticated users can access the application",
                Instance = ""
            };

            return new ForbiddenDto()
            {
                ApiVersion = "1.0",
                Id = Guid.NewGuid(),
                Method = "",
                Data = null,
                Error = error
            };
        }
    }
}
