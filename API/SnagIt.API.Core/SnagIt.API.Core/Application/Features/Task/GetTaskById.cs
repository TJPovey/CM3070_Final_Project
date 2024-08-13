using FluentValidation;
using MediatR;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Application.Models.Task;
using SnagIt.API.Core.Application.Extensions.Mapping.Tasks;
using SnagIt.API.Core.Domain.Aggregates.SnagItTask;


namespace SnagIt.API.Core.Application.Features.SnagTask
{
    public class GetTaskById
    {
        public class Query : IRequest<TaskDto.TaskDetailItem>
        {
            private Query(
                string username,
                Guid userId,
                Guid propertyId,
                Guid propertyOwnerId,
                Guid taskId)
            {
                Username = username;
                UserId = userId;
                PropertyId = propertyId;
                PropertyOwnerId = propertyOwnerId;
                TaskId = taskId;
            }

            public static Query Create(
                string username,
                Guid userId,
                Guid propertyId,
                Guid propertyOwnerId,
                Guid taskId)
                => new Query(username, userId, propertyId, propertyOwnerId, taskId);

            public string Username { get; }
            public Guid UserId { get; }
            public Guid PropertyId { get; }
            public Guid PropertyOwnerId { get; }
            public Guid TaskId { get; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(query => query.Username)
                    .NotEmpty();

                RuleFor(query => query.UserId)
                    .NotEmpty();

                RuleFor(query => query.PropertyId)
                    .NotEmpty();

                RuleFor(query => query.PropertyOwnerId)
                    .NotEmpty();

                RuleFor(query => query.TaskId)
                    .NotEmpty();
            }
        }

        public class AuthorisationHandler : IAuthoriseRequestPolicy<Query>
        {
            private readonly IMediator _mediator;

            public AuthorisationHandler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<AuthorisationResult> Authorise(Query request, CancellationToken cancellationToken)
            {
                var userIsAssignedToProperty = VerifyUserIsAssignedToTargetProperty.Query.Create(
                        request.UserId,
                        request.Username,
                        request.PropertyId);

                var userIsAssignedToPropertyResult = await _mediator.Send(userIsAssignedToProperty, cancellationToken);
                if (userIsAssignedToPropertyResult.IsAuthorised)
                {
                    return AuthorisationResult.Success();
                }

                return AuthorisationResult.Failure($"Only users assigned to the property are authorised to perform this action.");
            }
        }

        public class Handler : IRequestHandler<Query, TaskDto.TaskDetailItem>
        {
            private readonly IPropertyRepository _propertyRepository;

            public Handler(IPropertyRepository propertyRepository)
            {
                _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            }

            public async Task<TaskDto.TaskDetailItem> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var data = await _propertyRepository.GetTask(request.TaskId, request.PropertyOwnerId, cancellationToken);
                if (data is null)
                {
                    throw new ArgumentException(
                        $"A {nameof(SnagItTask)} projection could not be found " +
                        $"for {nameof(GetTaskById)} with taskId: {request.TaskId}");
                }

                var result = data.ToTaskDetailItem();

                return result;
            }
        }
    }
}
