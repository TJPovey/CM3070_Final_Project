using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Features.SnagTask;
using SnagIt.API.Core.Application.Models.Property;
using SnagIt.API.Core.Application.Models.Task;


namespace SnagIt.API.Core.Application.Features.Property.API
{
    public class TaskStatusPut
    {
        public class Command : IRequest<TaskDto>
        {
            private Command(Stream data, Guid taskId, Guid propertyId, Guid userId, string userName)
            {
                Data = data;
                UserId = userId;
                UserName = userName;
                PropertyId = propertyId;
                TaskId = taskId;
            }

            public static Command Create(Stream data, Guid taskId, Guid propertyId, Guid userId, string userName)
                => new Command(data, taskId, propertyId, userId, userName);

            public Stream Data { get; }
            public Guid UserId { get; }
            public Guid PropertyId { get; }
            public Guid TaskId { get; }
            public string UserName { get; }
        }

        public class Handler : IRequestHandler<Command, TaskDto>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<TaskDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request is null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                ResponseError error = null;
                try
                {
                    var requestBody = await new StreamReader(request.Data).ReadToEndAsync();
                    var dto = JsonConvert.DeserializeObject<TaskStatusPutDto>(requestBody);
                    var command = UpdateTaskOpenStatus.Command.Create(dto, request.TaskId, request.PropertyId, request.UserId, request.UserName);
                    await _mediator.Send(command, cancellationToken);
                }
                catch (Exception ex)
                {
                    if (ex is ValidationException
                        || ex is ArgumentException
                        || ex is JsonException)
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

                return new TaskDto
                {
                    ApiVersion = "1.0",
                    Method = "task.status.put",
                    Data = null,
                    Id = request.TaskId,
                    Error = error
                };
            }
        }
    }
}
