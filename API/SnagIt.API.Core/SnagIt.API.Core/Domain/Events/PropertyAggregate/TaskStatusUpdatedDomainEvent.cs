using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.SnagItTask;


namespace SnagIt.API.Core.Domain.Events.PropertyAggregate
{
    public class TaskStatusUpdatedDomainEvent : INotification
    {
        private TaskStatusUpdatedDomainEvent(SnagItTask snagItTask, UserId userId)
        {
            SnagItTask = snagItTask;
            OwnerId = userId;
        }

        public static TaskStatusUpdatedDomainEvent Create(SnagItTask snagItTask, UserId userId)
            => new TaskStatusUpdatedDomainEvent(snagItTask, userId);

        public SnagItTask SnagItTask { get; }
        public UserId OwnerId { get; }
    }
}
