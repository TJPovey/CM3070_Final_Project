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
    public class UserLogin
    {
        public class Command : IRequest<TokenDto>
        {
            private Command(Stream data)
            {
                Data = data;
            }

            public static Command Create(Stream data)
                => new Command(data);

            public Stream Data { get; }
        }

        public class Handler : IRequestHandler<Command, TokenDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<TokenDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                TokenDto.TokenDetailItem data = null;
                ResponseError error = null;
                try
                {
                    var requestBody = await new StreamReader(request.Data).ReadToEndAsync();
                    var dto = JsonConvert.DeserializeObject<TokenPostDto>(requestBody);

                    var command = GetTokenDetailByUsernameAndPassword.Query.Create(dto);

                    var result = await _mediator.Send(command, cancellationToken);

                    data = result;
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
                            Title = $"{nameof(AuthorisationException)}",
                            Status = 403,
                            Detail = "Incorrect username or password.",
                            Instance = ""
                        };
                    }
                    else
                    {
                        throw;
                    }
                }

                return new TokenDto
                {
                    ApiVersion = "1.0",
                    Method = "user.login",
                    Data = data,
                    Id = Guid.NewGuid(),
                    Error = error
                };
            }
        }
    }
}
