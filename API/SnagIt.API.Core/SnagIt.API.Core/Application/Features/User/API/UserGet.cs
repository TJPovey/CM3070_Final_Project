using MediatR;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using FluentValidation;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.User;
using SnagIt.API.Core.Application.Features.User;


namespace SnagIt.API.Core.Application.Features.Property.API
{
    public class UserGet
    {
        public class Query : IRequest<UserDto>
        {
            private Query(
                string username,
                Guid userId)
            {
                Username = username;
                UserId = userId;
            }

            public static Query Create(
                string username,
                Guid userId)
                => new Query(username, userId);

            public string Username { get; }

            public Guid UserId { get; }
        }

        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                UserDto.UserDetailItem data = null;
                ResponseError error = null;
                try
                {
                    var query = GetUserById.Query.Create(
                        request.Username,
                        request.UserId);
                    var result = await _mediator.Send(query, cancellationToken);

                    data = result;
                }
                catch (Exception ex)
                {
                    if (ex is ValidationException || 
                        ex is ArgumentException || 
                        ex is FormatException)
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

                var response = new UserDto()
                {
                    ApiVersion = "1.0",
                    Id = Guid.NewGuid(),
                    Method = "user.get",
                    Data = data,
                    Error = error
                };

                return response;
            }
        }
    }
}
