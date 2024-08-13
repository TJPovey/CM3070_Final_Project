﻿using FluentValidation;
using MediatR;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Infrastructure.Repositiories.Blob.Clients;


namespace SnagIt.API.Core.Application.Features.Property
{
    public class AssignImageToProperty
    {
        public class Command : IRequest
        {
            private Command(
                PropertyImageAssignmentPutDto data, 
                Guid propertyId, 
                Guid userId, 
                string userName)
            {
                Data = data;
                UserId = userId;
                Username = userName;
                PropertyId = propertyId;
            }

            public static Command Create(
                PropertyImageAssignmentPutDto data, 
                Guid propertyId, 
                Guid userId, 
                string userName)
                => new Command(data, propertyId, userId, userName);

            public PropertyImageAssignmentPutDto Data { get; }
            public Guid PropertyId { get; }
            public Guid UserId { get; }
            public string Username { get; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(query => query.Data)
                    .NotNull()
                    .SetValidator(new PropertyImageAssignmentPutDtoValidator());

                RuleFor(data => data.PropertyId)
                    .NotEmpty();

                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.Username)
                    .NotEmpty();
            }

            private class PropertyImageAssignmentPutDtoValidator : AbstractValidator<PropertyImageAssignmentPutDto>
            {
                public PropertyImageAssignmentPutDtoValidator()
                {
                    RuleFor(data => data.ImageName)
                        .NotEmpty();
                }
            }
        }

        public class AuthorisationHandler : IAuthoriseRequestPolicy<Command>
        {
            private readonly IMediator _mediator;

            public AuthorisationHandler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<AuthorisationResult> Authorise(Command request, CancellationToken cancellationToken)
            {
                var userIsAssignedToPropertyAsOwner =
                    VerifyUserIsAssignedToTargetPropertyAsOwner.Query.Create(
                        request.UserId,
                        request.Username,
                        request.PropertyId);

                var userIsAssignedToPropertyAsOwnerResult =
                    await _mediator.Send(userIsAssignedToPropertyAsOwner, cancellationToken);

                if (userIsAssignedToPropertyAsOwnerResult.IsAuthorised)
                {
                    return AuthorisationResult.Success();
                }

                var userIsAssignedToPropertyAsContributer =
                    VerifyUserIsAssignedToTargetPropertyAsContributer.Query.Create(
                        request.UserId,
                        request.Username,
                        request.PropertyId);

                var userIsAssignedToPropertyAsContributerResult =
                    await _mediator.Send(userIsAssignedToPropertyAsContributer, cancellationToken);

                if (userIsAssignedToPropertyAsContributerResult.IsAuthorised)
                {
                    return AuthorisationResult.Success();
                }

                return AuthorisationResult.Failure($"Only users assigned to the property as an owner or contributer are authorised to perform this action.");
            }
        }


        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            private readonly IPropertyRepository _propertyRepository;
            private readonly IUserRepository _userRepository;
            private readonly IIsolatedBlobClient _isolatedBlobClient;

            public Handler(
                IMediator mediator,
                IPropertyRepository propertyRepository,
                IUserRepository userRepository,
                IIsolatedBlobClient isolatedBlobClient)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
                _isolatedBlobClient = isolatedBlobClient ?? throw new ArgumentNullException(nameof(isolatedBlobClient));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                await VerifyUserExists(request, cancellationToken);
                var property = await VerifyPropertyExists(request, cancellationToken);

                await Create(request, property, cancellationToken);
            }

            private async Task Create(
                Command request,
                SnagItProperty property,
                CancellationToken cancellationToken)
            {
                var imageUri = await _isolatedBlobClient.GetPropertyImageReadToken(
                    property.OwnerId.Id,
                    property.Id,
                    request.Data.ImageName);

                if (imageUri is null) 
                {
                    throw new ArgumentNullException($"{nameof(imageUri)} is null");
                }

                property.AssignImageToProperty(imageUri);

                await _propertyRepository.UpdateProperty(property, request.UserId, cancellationToken);
            }

            private async Task VerifyUserExists(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetUser(request.UserId, request.Username, cancellationToken);
                if (user is null)
                {
                    throw new ArgumentException($"A {nameof(SnagItUser)} entity does not exists.");
                }
            }

            private async Task<SnagItProperty> VerifyPropertyExists(Command request, CancellationToken cancellationToken)
            {
                var property = await _propertyRepository.GetProperty(
                    request.PropertyId,
                    request.UserId,
                    cancellationToken);

                if (property is null)
                {
                    throw new ArgumentException($"A {nameof(SnagItProperty)} entity does not exists.");
                }

                return property;
            }
        }
    }
}
