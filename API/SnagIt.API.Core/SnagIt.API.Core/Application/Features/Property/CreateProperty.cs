using FluentValidation;
using MediatR;
using SnagIt.API.Core.Application.Features.Shared.Validators;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Infrastructure.Repositiories;


namespace SnagIt.API.Core.Application.Features.Property
{
    public class CreateProperty
    {
        public class Command : IRequest
        {
            private Command(PropertyPostDto data, Guid userId, string userName)
            {
                Data = data;
                UserId = userId;
                Username = userName;
            }

            public static Command Create(PropertyPostDto data, Guid userId, string userName)
                => new Command(data, userId, userName);

            public PropertyPostDto Data { get; }

            public Guid UserId { get; }

            public string Username { get; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(query => query.Data)
                    .NotNull()
                    .SetValidator(new PropertyPostDtoValidator());

                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.Username)
                    .NotEmpty();
            }

            private class PropertyPostDtoValidator : AbstractValidator<PropertyPostDto>
            {
                public PropertyPostDtoValidator()
                {
                    RuleFor(data => data.ReportTitle)
                        .NotEmpty();

                    RuleFor(data => data.PropertyName)
                        .NotEmpty();

                    RuleFor(data => data.Location)
                        .NotNull()
                        .SetValidator(new LocationPostValidator());
                }
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

                var user = await VerifyUserExists(request, cancellationToken);

                await Create(request, user, cancellationToken);
            }

            private async Task Create(
                Command request,
                SnagItUser user,
                CancellationToken cancellationToken)
            {
                var locationDetail = LocationDetail.FromDegrees(
                    request.Data.Location.Latitude,
                    request.Data.Location.Longitude);

                var propertyDetail = PropertyDetail.Create(
                    request.Data.PropertyName,
                    request.Data.ReportTitle,
                    null,
                    locationDetail);

                var propertyEntity = SnagItProperty
                    .Create(propertyDetail,
                            UserId.Create(user.Id, user.UserDetail.UserName));

                await _propertyRepository.AddProperty(propertyEntity, user.Id, cancellationToken);
            }

            private async Task<SnagItUser> VerifyUserExists(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetUser(request.UserId, request.Username, cancellationToken);
                if (user is null)
                {
                    throw new ArgumentException($"A {nameof(SnagItUser)} entity does not exists.");
                }

                return user;
            }
        }
    }
}
