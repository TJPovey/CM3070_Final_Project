using FluentValidation;
using MediatR;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.API;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Application.Extensions.Mapping.Property;


namespace SnagIt.API.Core.Application.Features.Property
{
    public class GetPropertyById
    {
        public class Query : IRequest<PropertyDto.PropertyDetailItem>
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

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.Username)
                    .NotEmpty();

                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.PropertyId)
                    .NotEmpty();
            }
        }

        public class AuthorisationHandler : IAuthoriseRequestPolicy<Query>
        {
            private readonly IMediator _mediator;

            public AuthorisationHandler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<AuthorisationResult> Authorise(Query request, CancellationToken cancellationToken)
            {
                var userIsAssignedToProperty = VerifyUserIsAssignedToTargetProperty.Query.Create(
                        request.UserId,
                        request.Username,
                        request.PropertyId);

                var userIsAssignedToPropertyResult = await _mediator.Send(userIsAssignedToProperty, cancellationToken);
                if (userIsAssignedToPropertyResult.IsAuthorised)
                {
                    return AuthorisationResult.Success();
                }

                return AuthorisationResult.Failure($"Only authenticated Platform Administrators can access the system.");
            }
        }

        public class Handler : IRequestHandler<Query, PropertyDto.PropertyDetailItem>
        {
            private readonly IPropertyRepository _propertyRepository;

            public Handler(IPropertyRepository propertyRepository)
            {
                _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            }

            public async Task<PropertyDto.PropertyDetailItem> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var data = await _propertyRepository.GetProperty(request.PropertyId, request.UserId, cancellationToken);
                if (data is null)
                {
                    throw new ArgumentException(
                        $"A {nameof(SnagItProperty)} projection could not be found " +
                        $"for {nameof(GetPropertyById)} with propertyId: {request.PropertyId}");
                }

                var result = data.ToPropertyDetailItem();

                return result;
            }
        }
    }
}
