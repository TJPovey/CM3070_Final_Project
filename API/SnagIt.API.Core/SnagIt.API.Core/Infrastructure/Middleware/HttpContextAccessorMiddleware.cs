using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;

namespace SnagIt.API.Core.Infrastructure.Middleware
{
    // Required as isolated worker model doesn't provide this context
    // https://github.com/Azure/azure-functions-dotnet-worker/issues/2372
    public class HttpContextAccessorMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessorMiddleware(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            _httpContextAccessor.HttpContext = context.GetHttpContext();
            await next(context);
        }
    }
}
