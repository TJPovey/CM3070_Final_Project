using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Events.PropertyAggregate;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using System.Threading.Tasks;


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

        public static SnagItTask Create(Guid taskId, TaskDetail taskDetail, UserId ownerId)
        {
            var task = new SnagItTask(taskId, taskDetail);

            var @event = TaskCreatedDomainEvent.Create(task, ownerId);

            task.AddDomainEvent(@event);

            return task;
        }

        public void UpdateStatus(bool open, UserId propertyOwnerId)
        {
            TaskDetail.UpdateOpenStatus(open);
            
            var @event = TaskStatusUpdatedDomainEvent.Create(this, propertyOwnerId);

            AddDomainEvent(@event);
        }

        public void AssignImageToTask(string imagePath)
        {
            if (imagePath is null)
            {
                throw new DomainException($"A {nameof(imagePath)} instance was not supplied.");
            }

            TaskDetail.UpdateImageUri(imagePath);
        }


        public static string GeneratePartitionKey(Guid taskId)
            => $"{PartitionKeyPrefix}{taskId}";


        public TaskDetail TaskDetail { get; private set; }
    }
}
