using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SnagIt.API.Core.Application.Features.Property.API;
using SnagIt.API.Core.Infrastructure.Extensions;

namespace SnagIt.API.Core.API
{
    public class SnagItPropertyAPI
    {
        private readonly IMediator _mediator;

        public SnagItPropertyAPI(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("API_Property_Post")]
        public async Task<IActionResult> PostProperty(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var claims = request.Headers.GetClaimsPrincipal();
            var userId = claims.GetUserId();
            var userName = claims.GetUserName();

            var command = PropertyPost.Command.Create(
                request.Body,
                userId,
                userName);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }

        [Function("API_Property_Assign_User")]
        public async Task<IActionResult> AssignUserToProperty(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var claims = request.Headers.GetClaimsPrincipal();
            var userId = claims.GetUserId();
            var userName = claims.GetUserName();
            var parameters = request.Query;
            var propertyId = parameters.Get("propertyId");

            var command = UserAssignmentPut.Command.Create(
                request.Body,
                Guid.Parse(propertyId),
                userId,
                userName);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }

        [Function("API_Property_Assign_Image")]
        public async Task<IActionResult> AssignImageToProperty(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var claims = request.Headers.GetClaimsPrincipal();
            var userId = claims.GetUserId();
            var userName = claims.GetUserName();
            var parameters = request.Query;
            var propertyId = parameters.Get("propertyId");

            var command = PropertyImagePut.Command.Create(
                request.Body,
                Guid.Parse(propertyId),
                userId,
                userName);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }

        [Function("API_Property_Get")]
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

            var query = PropertyGet.Query.Create(
                userName,
                userId,
                Guid.Parse(propertyId));

            var result = await _mediator.Send(query, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }
    }
}
