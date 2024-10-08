﻿using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos.Linq;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Application.Features.Shared.Validators;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Infrastructure.Repositiories;


namespace SnagIt.API.Core.Application.Features.Property
{
    public class AssignUserToProperty
    {
        public class Command : IRequest
        {
            private Command(PropertyUserAssignmentPutDto data, Guid propertyId, Guid userId, string userName)
            {
                Data = data;
                UserId = userId;
                Username = userName;
                PropertyId = propertyId;
            }

            public static Command Create(PropertyUserAssignmentPutDto data, Guid propertyId, Guid userId, string userName)
                => new Command(data, propertyId, userId, userName);

            public PropertyUserAssignmentPutDto Data { get; }
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
                    .SetValidator(new PropertyUserAssignmentPostDtoValidator());

                RuleFor(data => data.PropertyId)
                    .NotEmpty();

                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.Username)
                    .NotEmpty();
            }

            private class PropertyUserAssignmentPostDtoValidator : AbstractValidator<PropertyUserAssignmentPutDto>
            {
                public PropertyUserAssignmentPostDtoValidator()
                {
                    RuleFor(data => data.AssignedToUserName)
                        .NotEmpty();

                    RuleFor(data => data.UserRole)
                        .NotEmpty()
                        .Must(role => 
                            UserRole.FromName(role) == UserRole.Contributer ||
                            UserRole.FromName(role) == UserRole.Reader)
                        .WithMessage($"UserRole must be either {UserRole.Contributer} or {UserRole.Reader}.");
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

                return AuthorisationResult.Failure($"Only users assigned to the property as an owner are authorised to perform this action.");
            }
        }


        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            private readonly IPropertyRepository _propertyRepository;
            private readonly IUserRepository _userRepository;

            public Handler(
                IMediator mediator,
                IPropertyRepository propertyRepository,
                IUserRepository userRepository)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                await VerifyUserExists(request, cancellationToken);
                var userToAssign = await VerifyAssignedUserExists(request, cancellationToken);
                var property = await VerifyPropertyExists(request, cancellationToken);

                await Create(request, userToAssign, property, cancellationToken);
            }

            private async Task Create(
                Command request,
                SnagItUser userToAssign,
                SnagItProperty property,
                CancellationToken cancellationToken)
            {
                var user = UserId.Create(userToAssign.Id, userToAssign.UserDetail.UserName);
                var role = UserRole.FromName(request.Data.UserRole);
                property.AssignUserToPropertyWithRole(user, role);

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

            private async Task<SnagItUser> VerifyAssignedUserExists(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAllUsersWithUsername(
                    request.Data.AssignedToUserName,
                    cancellationToken);

                if (user != null && user?.Count == 0)
                {
                    throw new ArgumentNullException($"A {nameof(SnagItUser)} user with username [${request.Data.AssignedToUserName}] could not be found.");
                }
                if (user?.Count > 1)
                {
                    throw new DomainException($"A {nameof(SnagItUser)} multiple users with the same username exist.");
                }
                return user.First();
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
