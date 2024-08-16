using FluentValidation;
using MediatR;
using NodaTime;
using NodaTime.Text;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Application.Features.Shared.Validators;
using SnagIt.API.Core.Application.Models.Task;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.SnagItTask;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Infrastructure.Repositiories;


namespace SnagIt.API.Core.Application.Features.SnagTask
{
    public class CreateTask
    {
        public class Command : IRequest
        {
            private Command(TaskPostDto data, Guid userId, string userName)
            {
                Data = data;
                UserId = userId;
                Username = userName;
            }

            public static Command Create(TaskPostDto data, Guid userId, string userName)
                => new Command(data, userId, userName);

            public TaskPostDto Data { get; }

            public Guid UserId { get; }

            public string Username { get; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(query => query.Data)
                    .NotNull()
                    .SetValidator(new TaskPostDtoValidator());

                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.Username)
                    .NotEmpty();
            }

            private class TaskPostDtoValidator : AbstractValidator<TaskPostDto>
            {
                public TaskPostDtoValidator()
                {
                    RuleFor(data => data.PropertyId)
                        .NotEmpty();

                    RuleFor(data => data.UserPropertyOwnerId)
                        .NotEmpty();

                    RuleFor(data => data.Title)
                        .NotEmpty();

                    RuleFor(data => data.Area)
                        .NotEmpty();

                    RuleFor(data => data.Description)
                        .NotEmpty();

                    RuleFor(data => data.DueDate)
                        .NotEmpty();

                    RuleFor(data => data.EstimatedCost)
                        .NotEmpty();

                    RuleFor(data => data.Category)
                        .NotEmpty();

                    RuleFor(data => data.Priority)
                        .NotEmpty();

                    RuleFor(data => data.AssignedToUserId)
                        .NotEmpty();

                    RuleFor(data => data.AssignedToUserUserName)
                        .NotEmpty();

                    RuleFor(data => data.Location)
                        .SetValidator(new LocationPostValidator())
                        .When(data => data.Location is not null);
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
                        request.Data.PropertyId);

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
                        request.Data.PropertyId);

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

                _ = await VerifyUserExists(request, cancellationToken);
                _ = await VerifyAssignedUserExists(request, cancellationToken);
                var property = await VerifyPropertyExists(request, cancellationToken);

                await Create(request, property, cancellationToken);
            }

            private async Task Create(
                Command request,
                SnagItProperty property,
                CancellationToken cancellationToken)
            {
                var taskCategory = TaskCategory.FromName(request.Data.Category);
                var taskPriority = TaskPriority.FromName(request.Data.Priority);

                var locationDetail = request.Data.Location is null ? 
                        null : 
                        LocationDetail.FromDegrees(
                            request.Data.Location.Latitude,
                            request.Data.Location.Longitude,
                            request.Data.Location.Elevation);

                var propertyId = PropertyId.Create(
                    property.Id, 
                    property.PropertyDetail.PropertyName);

                var assignedUser = UserId.Create(
                    request.Data.AssignedToUserId,
                    request.Data.AssignedToUserUserName);

                var date = InstantPattern.ExtendedIso.Parse(request.Data.DueDate).Value;

                var taskDetail = TaskDetail.Create(
                    request.Data.Title,
                    request.Data.Area,
                    request.Data.Description,
                    true,
                    date,
                    request.Data.EstimatedCost,
                    taskCategory,
                    taskPriority,
                    locationDetail,
                    propertyId,
                    assignedUser);

                var taskEntity = SnagItTask.Create(taskDetail, property.OwnerId);

                await _propertyRepository.AddTask(
                    taskEntity, 
                    request.Data.UserPropertyOwnerId, 
                    cancellationToken);
            }

            private async Task<SnagItUser> VerifyUserExists(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetUser(
                    request.UserId, 
                    request.Username, 
                    cancellationToken);

                if (user is null)
                {
                    throw new ArgumentException($"A {nameof(SnagItUser)} entity does not exists.");
                }

                return user;
            }

            private async Task<SnagItUser> VerifyAssignedUserExists(Command request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetUser(
                    request.Data.AssignedToUserId,
                    request.Data.AssignedToUserUserName,
                    cancellationToken);

                if (user is null)
                {
                    throw new ArgumentException($"A {nameof(SnagItUser)} entity does not exists to assign to the task.");
                }

                return user;
            }

            private async Task<SnagItProperty> VerifyPropertyExists(Command request, CancellationToken cancellationToken)
            {
                var property = await _propertyRepository.GetProperty(
                    request.Data.PropertyId,
                    request.Data.UserPropertyOwnerId,
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
