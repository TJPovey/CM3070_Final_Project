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

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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

                var tokenDescriptor = GenerateToken(user);

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

            private SecurityTokenDescriptor GenerateToken(SnagItUser user)
            {
                var signingKey = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
                var key = Encoding.ASCII.GetBytes(signingKey);

                var claims = new Dictionary<string, object>
                    {
                        { "userId", user.Id.ToString() },
                        { "userName", user.UserDetail.UserName },
                        { "email", user.UserDetail.Email }
                    };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature),
                    Claims = claims,
                    Audience = "postman",
                    Issuer = "SnagItApp",
                    TokenType = "Bearer"
                };

                return tokenDescriptor;
            }
        }
    }
}
