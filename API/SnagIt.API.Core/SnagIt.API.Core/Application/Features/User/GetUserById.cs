using FluentValidation;
using MediatR;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Application.Extensions.Mapping.User;
using SnagIt.API.Core.Application.Models.User;


namespace SnagIt.API.Core.Application.Features.User
{
    public class GetUserById
    {
        public class Query : IRequest<UserDto.UserDetailItem>
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

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.Username)
                    .NotEmpty();

                RuleFor(query => query.UserId)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, UserDto.UserDetailItem>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public async Task<UserDto.UserDetailItem> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var data = await _userRepository.GetUser(
                    request.UserId,
                    request.Username,
                    cancellationToken);

                var result = data.ToUserDetailItem();

                return result;
            }
        }
    }
}
