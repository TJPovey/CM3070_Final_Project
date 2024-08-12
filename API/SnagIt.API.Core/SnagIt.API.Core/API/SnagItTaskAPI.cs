using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SnagIt.API.Core.Application.Features.Property.API;
using SnagIt.API.Core.Application.Features.SnagTask.API;
using SnagIt.API.Core.Infrastructure.Extensions;

namespace SnagIt.API.Core.API
{
    public class SnagItTaskAPI
    {
        private readonly IMediator _mediator;

        public SnagItTaskAPI(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("API_Task_Post")]
        public async Task<IActionResult> PostTask(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var claims = request.Headers.GetClaimsPrincipal();
            var userId = claims.GetUserId();
            var userName = claims.GetUserName();

            var command = TaskPost.Command.Create(
                request.Body,
                userId,
                userName);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }

        [Function("API_Task_Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var claims = request.Headers.GetClaimsPrincipal();
            var userId = claims.GetUserId();
            var userName = claims.GetUserName();

            var parameters = request.Query;
            var propertyId = parameters.Get("propertyId");
            var taskId = parameters.Get("taskId");
            var propertyOwnerId = parameters.Get("propertyOwnerId");

            var query = TaskGet.Query.Create(
                userName,
                userId,
                Guid.Parse(propertyId),
                Guid.Parse(propertyOwnerId),
                Guid.Parse(taskId));

            var result = await _mediator.Send(query, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }
    }
}
