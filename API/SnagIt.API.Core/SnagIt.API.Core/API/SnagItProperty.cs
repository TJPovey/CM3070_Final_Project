using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SnagIt.API.Core.Application.Features.User.API;
using SnagIt.API.Core.Infrastructure.Extensions;

namespace SnagIt.API.Core.API
{
    public class SnagItProperty
    {
        private readonly IMediator _mediator;

        public SnagItProperty(IMediator mediator)
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
            var command = UserPost.Command.Create(request.Body);

            var result = await _mediator.Send(command, cancellationToken);

            return result.GenerateIActionResultForResponse();
        }
    }
}
