using MediatR;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using FluentValidation;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.Property;


namespace SnagIt.API.Core.Application.Features.Property.API
{
    public class PropertyGet
    {
        public class Query : IRequest<PropertyDto>
        {
            private Query(
                string username,
                Guid userId,
                Guid propertyId)
            {
                Username = username;
                UserId = userId;
                PropertyId = propertyId;
            }

            public static Query Create(
                string username,
                Guid userId,
                Guid propertyId)
                => new Query(username, userId, propertyId);

            public string Username { get; }

            public Guid UserId { get; }

            public Guid PropertyId { get; }
        }

        public class Handler : IRequestHandler<Query, PropertyDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<PropertyDto> Handle(Query request, CancellationToken cancellationToken)
            {
                PropertyDto.PropertyDetailItem data = null;
                ResponseError error = null;
                try
                {
                    var query = GetPropertyById.Query.Create(
                        request.Username,
                        request.UserId,
                        request.PropertyId);
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

                var response = new PropertyDto()
                {
                    ApiVersion = "1.0",
                    Id = request.PropertyId,
                    Method = "property.get",
                    Data = data,
                    Error = error
                };

                return response;
            }
        }
    }
}
