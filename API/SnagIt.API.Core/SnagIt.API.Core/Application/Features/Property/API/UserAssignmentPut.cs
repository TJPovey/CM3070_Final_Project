using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.Property;


namespace SnagIt.API.Core.Application.Features.Property.API
{
    public class UserAssignmentPut
    {
        public class Command : IRequest<PropertyDto>
        {
            private Command(Stream data, Guid propertyId, Guid userId, string userName)
            {
                Data = data;
                UserId = userId;
                UserName = userName;
                PropertyId = propertyId;
            }

            public static Command Create(Stream data, Guid propertyId, Guid userId, string userName)
                => new Command(data, propertyId, userId, userName);

            public Stream Data { get; }

            public Guid UserId { get; }
            public Guid PropertyId { get; }

            public string UserName { get; }
        }

        public class Handler : IRequestHandler<Command, PropertyDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<PropertyDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                ResponseError error = null;
                try
                {
                    var requestBody = await new StreamReader(request.Data).ReadToEndAsync();
                    var dto = JsonConvert.DeserializeObject<PropertyUserAssignmentPutDto>(requestBody);
                    var command = AssignUserToProperty.Command.Create(dto, request.PropertyId, request.UserId, request.UserName);
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

                return new PropertyDto
                {
                    ApiVersion = "1.0",
                    Method = "user.property.put",
                    Data = null,
                    Id = Guid.NewGuid(),
                    Error = error
                };
            }
        }
    }
}
