using MediatR;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Application.Models.User;
using FluentValidation;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.User;

namespace SnagIt.API.Core.Application.Features.User
{
    public class CreateUser
    {
        public class Command : IRequest
        {
            private Command(UserPostDto data)
            {
                Data = data;
            }

            public static Command Create(UserPostDto data)
                => new Command(data);

            public UserPostDto Data { get; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(query => query.Data)
                    .NotNull()
                    .SetValidator(new UserPostDtoValidator());
            }

            private class UserPostDtoValidator : AbstractValidator<UserPostDto>
            {
                public UserPostDtoValidator()
                {
                    RuleFor(data => data.Email)
                        .NotEmpty()
                            .WithMessage("Email is required.")
                        .EmailAddress()
                            .WithMessage("Invalid email address format.");

                    RuleFor(data => data.FirstName)
                        .NotEmpty();

                    RuleFor(data => data.LastName)
                        .NotEmpty();

                    RuleFor(data => data.Username)
                        .NotEmpty()
                            .WithMessage("Username is required.")
                        .MinimumLength(3)
                            .WithMessage("Username must be at least 3 characters long.")
                        .MaximumLength(20)
                            .WithMessage("Username must not exceed 20 characters.")
                        .Matches("^[a-zA-Z0-9_]*$")
                            .WithMessage("Username can only contain letters, numbers, and underscores.");

                    RuleFor(data => data.Password)
                        .NotEmpty()
                            .WithMessage("Password is required.")
                        .MinimumLength(8)
                            .WithMessage("Password must be at least 8 characters long.")
                        .Matches(@"[A-Z]")
                            .WithMessage("Password must contain at least one uppercase letter.")
                        .Matches(@"[a-z]")
                            .WithMessage("Password must contain at least one lowercase letter.")
                        .Matches(@"[0-9]")
                            .WithMessage("Password must contain at least one number.")
                        .Matches(@"[\!\@\#\$\%\^\&\*\(\)\_\+\-]")
                            .WithMessage("Password must contain at least one special character.");

                }
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;
            private readonly IUserRepository _userRepository;

            public Handler(
                IMediator mediator,
                IUserRepository userRepository)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                await VerifyUserDoesNotExist(request, cancellationToken);

                await Create(request, cancellationToken);
            }

            private async Task VerifyUserDoesNotExist(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAllUsersWithUsername(request.Data.Username, cancellationToken);
                if (user?.Count > 0)
                {
                    throw new ArgumentException($"A {nameof(Domain.Aggregates.User.SnagItUser)} entity already exists.");
                }
            }

            private async Task Create(
                Command request,
                CancellationToken cancellationToken)
            {
                var userDetail = UserDetail.Create(
                    request.Data.FirstName,
                    request.Data.LastName,
                    request.Data.Username,
                    request.Data.Email);

                var snagItEntity = SnagItUser.Create(request.Data.Password, userDetail);

                await _userRepository.AddUser(snagItEntity, cancellationToken);
            }
        }
    }
}
