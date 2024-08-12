using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SnagIt.API.Core.Application.Exceptions;
using SnagIt.API.Core.Application.Extensions.Exceptions;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.Task;


namespace SnagIt.API.Core.Application.Features.SnagTask.API
{
    public class TaskPost
    {
        public class Command : IRequest<TaskDto>
        {
            private Command(Stream data, Guid userId, string userName)
            {
                Data = data;
                UserId = userId;
                UserName = userName;
            }

            public static Command Create(Stream data, Guid userId, string userName)
                => new Command(data, userId, userName);

            public Stream Data { get; }

            public Guid UserId { get; }

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
                    var dto = JsonConvert.DeserializeObject<TaskPostDto>(requestBody);
                    var command = CreateTask.Command.Create(dto, request.UserId, request.UserName);
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
                    Method = "task.post",
                    Data = null,
                    Id = Guid.NewGuid(),
                    Error = error
                };
            }
        }
    }
}
