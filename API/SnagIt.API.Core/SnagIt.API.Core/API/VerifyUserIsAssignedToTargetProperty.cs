﻿using FluentValidation;
using MediatR;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Infrastructure.Repositiories;


namespace SnagIt.API.Core.API
{
    public class VerifyUserIsAssignedToTargetProperty
    {
        public class Query : IRequest<AuthorisationResult>
        {
            private Query(Guid userId, string username, Guid propertyId)
            {
                UserId = userId;
                Username = username;
                PropertyId = propertyId;
            }

            public static Query Create(Guid userId, string username, Guid propertyId)
                    => new Query(userId, username, propertyId);

            public Guid UserId { get; }
            public string Username { get; }
            public Guid PropertyId { get; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.PropertyId)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, AuthorisationResult>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public async Task<AuthorisationResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var userProfile = await _userRepository.GetUser(request.UserId, request.Username, cancellationToken);
                if (userProfile is not null && userProfile.AssignedProperties
                    .Any(x => x.Property.Id.Equals(request.PropertyId)))
                {
                    return AuthorisationResult.Success();
                }

                return AuthorisationResult.Failure($"User is not assigned to the target Property [{request.PropertyId}].");
            }
        }
    }
}
