using FluentValidation;
using MediatR;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Application.Models.User;
using SnagIt.API.Core.Infrastructure.Repositiories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Application.Features.Property
{
    public class CreateProperty
    {
        public class Command : IRequest
        {
            private Command(PropertyPostDto data)
            {
                Data = data;
            }

            public static Command Create(PropertyPostDto data)
                => new Command(data);

            public PropertyPostDto Data { get; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(query => query.Data)
                    .NotNull()
                    .SetValidator(new PropertyPostDtoValidator());
            }

            private class PropertyPostDtoValidator : AbstractValidator<PropertyPostDto>
            {
                public PropertyPostDtoValidator()
                {
                    RuleFor(data => data.ReportTitle)
                        .NotEmpty();

                    RuleFor(data => data.PropertyName)
                        .NotEmpty();
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

                //await VerifyUserDoesNotExist(request, cancellationToken);

                //await Create(request, cancellationToken);
            }

            //private async Task VerifyUserDoesNotExist(Command request, CancellationToken cancellationToken)
            //{
            //    var user = await _userRepository.GetAll(request.Data.Username, cancellationToken);
            //    if (user?.Count > 0)
            //    {
            //        throw new ArgumentException($"A {nameof(Domain.Aggregates.User.SnagItUser)} entity already exists.");
            //    }
            //}

            //private async Task Create(
            //    Command request,
            //    CancellationToken cancellationToken)
            //{
            //    var userDetail = UserDetail.Create(
            //        request.Data.FirstName,
            //        request.Data.LastName,
            //        request.Data.Username,
            //        request.Data.Email);

            //    var snagItEntity = Domain.Aggregates.User.SnagItUser
            //        .Create(
            //            Guid.NewGuid(),
            //            request.Data.Password,
            //            userDetail);

            //    await _userRepository.Add(snagItEntity, cancellationToken);
            //}
        }
    }
}
