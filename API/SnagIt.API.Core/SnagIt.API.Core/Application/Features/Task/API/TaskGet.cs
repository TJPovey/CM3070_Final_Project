using MediatR;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using FluentValidation;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Application.Models.Task;


namespace SnagIt.API.Core.Application.Features.SnagTask.API
{
    public class TaskGet
    {
        public class Query : IRequest<TaskDto>
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

        public class Handler : IRequestHandler<Query, TaskDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<TaskDto> Handle(Query request, CancellationToken cancellationToken)
            {
                TaskDto.TaskDetailItem data = null;
                ResponseError error = null;
                try
                {
                    var query = GetTaskById.Query.Create(
                        request.Username,
                        request.UserId,
                        request.PropertyId,
                        request.PropertyOwnerId,
                        request.TaskId);
                    var result = await _mediator.Send(query, cancellationToken);

                    data = result;
                }
                catch (Exception ex)
                {
                    if (ex is ValidationException || 
                        ex is ArgumentException || 
                        ex is FormatException)
                    {
                        error = new ResponseError
                        {
                            Type = "",
                            Title = ex.GetTitle(),
                            Status = 400,
                            Detail = ex.GetErrorDetail(),
                            Instance = ""
                        };
                    }
                    else if (ex is AuthorisationException)
                    {
                        error = new ResponseError
                        {
                            Type = "",
                            Title = "",
                            Status = 403,
                            Detail = "",
                            Instance = ""
                        };
                    }
                    else
                    {
                        throw;
                    }
                }

                var response = new TaskDto()
                {
                    ApiVersion = "1.0",
                    Id = Guid.NewGuid(),
                    Method = "task.get",
                    Data = data,
                    Error = error
                };

                return response;
            }
        }
    }
}
