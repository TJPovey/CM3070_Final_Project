using MediatR;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using SnagIt.API.Core.Application.Models.User;
using System.ComponentModel.DataAnnotations;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using SnagIt.API.Core.Application.Exceptions;

namespace SnagIt.API.Core.Application.Features.User.API
{
    public class UserPost
    {
        public class Command : IRequest<UserDto>
        {
            private Command(Stream data)
            {
                Data = data;
            }

            public static Command Create(Stream data)
                => new Command(data);

            public Stream Data { get; }
        }

        public class Handler : IRequestHandler<Command, UserDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                ResponseError error = null;
                try
                {
                    var requestBody = await new StreamReader(request.Data).ReadToEndAsync();
                    var dto = JsonConvert.DeserializeObject<UserPostDto>(requestBody);
                    var command = CreateUser.Command.Create(dto);
                    await _mediator.Send(command, cancellationToken);
                }
                catch (Exception ex)
                {
                    if (ex is ValidationException
                        || ex is ArgumentException
                        || ex is JsonException)
                    {
                        error = new ResponseError
                        {
                            Type = "",
                            Title = ex.GetTitle(),
                            Status = 400,
                            Detail = ex.GetErrorDetail(),
                            Instance = ""
                        };
                    }
                    else if (ex is AuthorisationException)
                    {
                        error = new ResponseError
                        {
                            Type = "",
                            Title = "",
                            Status = 403,
                            Detail = "",
                            Instance = ""
                        };
                    }
                    else
                    {
                        throw;
                    }
                }

                return new UserDto
                {
                    ApiVersion = "1.0",
                    Method = "user.post",
                    Data = null,
                    Id = Guid.NewGuid(),
                    Error = error
                };
            }
        }
    }
}
