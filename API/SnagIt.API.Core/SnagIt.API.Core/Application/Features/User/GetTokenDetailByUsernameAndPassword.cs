using FluentValidation;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Mapping;
using SnagIt.API.Core.Application.Models.User;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Infrastructure.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static SnagIt.API.Core.Application.Models.User.TokenDto;

namespace SnagIt.API.Core.Application.Features.User
{
    public class GetTokenDetailByUsernameAndPassword
    {
        public class Query : IRequest<TokenDto.TokenDetailItem>
        {
            private Query(TokenPostDto data)
            {
                Data = data;
            }

            public static Query Create(TokenPostDto data)
                => new Query(data);

            public TokenPostDto Data { get; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.Data)
                    .NotNull()
                    .SetValidator(new TokenPostDtoValidator());
            }
        }

        private class TokenPostDtoValidator : AbstractValidator<TokenPostDto>
        {
            public TokenPostDtoValidator()
            {
                RuleFor(query => query.Username)
                    .NotNull();

                RuleFor(query => query.Password)
                    .NotNull();
            }
        }

        public class Handler : IRequestHandler<Query, TokenDto.TokenDetailItem>
        {
            private readonly IUserRepository _userRepository;
            private readonly IJwtSecurityTokenHandlerService _jwtSecurityTokenHandlerService;

            public Handler(
                IUserRepository userRepository,
                IJwtSecurityTokenHandlerService jwtSecurityTokenHandlerService)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
                _jwtSecurityTokenHandlerService = jwtSecurityTokenHandlerService ?? throw new ArgumentNullException(nameof(jwtSecurityTokenHandlerService));
            }

            public async Task<TokenDto.TokenDetailItem> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var user = await GetUser(request, cancellationToken);
                var passwordHasher = new PasswordHasher<SnagItUser>();

                var passwordVerified = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Data.Password);

                if (!passwordVerified.Equals(PasswordVerificationResult.Success))
                {
                    throw new AuthorisationException();
                }

                var tokenDescriptor = _jwtSecurityTokenHandlerService.GenerateToken(
                    user.Id, user.UserDetail.UserName, user.UserDetail.Email);

                return tokenDescriptor.ToTokenDetailtem(user);
            }

            private async Task<SnagItUser> GetUser(Query request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAll(request.Data.Username, cancellationToken);
                if (user != null && user?.Count == 0)
                {
                    throw new ArgumentNullException($"A {nameof(SnagItUser)} user with username [${request.Data.Username}] could not be found.");
                }
                if (user?.Count > 1)
                {
                    throw new DomainException($"A {nameof(SnagItUser)} multiple users with the same username exist.");
                }
                return user.First();
            }
        }
    }
}
