using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.SnagItTask;


namespace SnagIt.API.Core.Domain.Events.PropertyAggregate
{
    public class TaskCreatedDomainEvent : INotification
    {
        private TaskCreatedDomainEvent(SnagItTask snagItTask, UserId userId)
        {
            SnagItTask = snagItTask;
            OwnerId = userId;
        }

        public static TaskCreatedDomainEvent Create(SnagItTask snagItTask, UserId userId)
            => new TaskCreatedDomainEvent(snagItTask, userId);

        public SnagItTask SnagItTask { get; }
        public UserId OwnerId { get; }
    }
}
