using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SnagIt.API.Core.Application.Features.User.API;
using SnagIt.API.Core.Infrastructure.Extensions;

namespace SnagIt.API.Core.API
{
    public class SnagItUserAPI
    {
        private readonly IMediator _mediator;

        public SnagItUserAPI(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("API_User_Post")]
        public async Task<IActionResult> PostUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var command = UserPost.Command.Create(request.Body);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }

        [Function("API_User_Login")]
        public async Task<IActionResult> LoginUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequestData request,
            FunctionContext context,
            CancellationToken cancellationToken)
        {
            var command = UserLogin.Command.Create(request.Body);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }
    }
}
