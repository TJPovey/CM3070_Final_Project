using FluentValidation;
using MediatR;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Application.Extensions.Mapping.Property;
using SnagIt.API.Core.Infrastructure.Repositiories.Blob.Clients;
using SnagIt.API.Core.Domain.Aggregates.Shared;


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

                return AuthorisationResult.Failure($"Only users assigned to the property are authorised to perform this action.");
            }
        }

        public class Handler : IRequestHandler<Query, PropertyDto.PropertyDetailItem>
        {
            private readonly IPropertyRepository _propertyRepository;
            private readonly IIsolatedBlobClient _isolatedBlobClient;

            public Handler(IPropertyRepository propertyRepository, IIsolatedBlobClient isolatedBlobClient)
            {
                _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
                _isolatedBlobClient = isolatedBlobClient ?? throw new ArgumentNullException(nameof(isolatedBlobClient));
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

                var userRole = data.AssignedUsers.First(x => x.UserId.Id.Equals(request.UserId)).Role;

                // Readers of this property are not authorised to gain a write token
                var writeToken = userRole.Equals(UserRole.Reader) ? 
                    null :
                    await _isolatedBlobClient.GetWriteToken(data.OwnerId.Id);

                var result = data.ToPropertyDetailItem(writeToken);

                return result;
            }
        }
    }
}
