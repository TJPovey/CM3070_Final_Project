using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.User
{
    public class TaskAssignment : ValueObject
    {
        [JsonConstructor]
        private TaskAssignment(TaskId taskId)
        {
            TaskId = taskId ?? throw new ArgumentNullException(nameof(taskId));
        }

        public static TaskAssignment Create(TaskId taskId)
            => new TaskAssignment(taskId);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TaskId;
        }

        public TaskId TaskId { get; }
    }
}
