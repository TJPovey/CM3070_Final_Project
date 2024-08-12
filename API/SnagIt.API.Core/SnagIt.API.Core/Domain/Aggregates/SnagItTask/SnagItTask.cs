using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Events.PropertyAggregate;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.SnagItTask
{
    public class SnagItTask : Entity, IAggregateRoot
    {
        public static string PartitionKeyPrefix = "task-";

        [Obsolete("This only exists for JSON deserialisation. Do not use it for any other purpose.")]
        [JsonConstructor]
        private SnagItTask(
            Guid id,
            string partitionKey,
            string type,
            string passwordHash,
            TaskDetail taskDetail)
        {
            Id = id;
            PartitionKey = partitionKey;
            Type = type;
            TaskDetail = taskDetail;
        }

        private SnagItTask(
            Guid id,
            TaskDetail taskDetail)
        {
            Id = !id.Equals(default) ? id : throw new DomainException(nameof(id));
            PartitionKey = GeneratePartitionKey(Id);
            Type = nameof(SnagItTask);
            TaskDetail = taskDetail ?? throw new DomainException(nameof(taskDetail));
        }

        public static SnagItTask Create(TaskDetail taskDetail, UserId ownerId)
        {
            var task = new SnagItTask(Guid.NewGuid(), taskDetail);

            var @event = TaskCreatedDomainEvent.Create(task, ownerId);

            task.AddDomainEvent(@event);

            return task;
        }

        public static string GeneratePartitionKey(Guid taskId)
            => $"{PartitionKeyPrefix}{taskId}";


        public TaskDetail TaskDetail { get; private set; }
    }
}
