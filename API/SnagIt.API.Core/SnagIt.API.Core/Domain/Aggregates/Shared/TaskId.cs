using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class TaskId : ValueObject
    {
        [JsonConstructor]
        private TaskId(
            Guid id, 
            string name,
            bool open,
            TaskCategory taskCategory,
            TaskPriority taskPriority,
            LocationDetail? locationDetail)
        {
            Id = !id.Equals(default) ? id : throw new DomainException($"A value for {nameof(id)} was not supplied.");
            Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new DomainException($"A value for {nameof(name)} was not supplied.");
            Open = open;
            TaskCategory = taskCategory ?? throw new DomainException($"A value for {nameof(taskCategory)} was not supplied.");
            TaskPriority = taskPriority ?? throw new DomainException($"A value for {nameof(taskPriority)} was not supplied.");
            LocationDetail = locationDetail;
        }

        public static TaskId Create(
            Guid id,
            string name,
            bool open,
            TaskCategory taskCategory,
            TaskPriority taskPriority,
            LocationDetail? locationDetail)
            => new TaskId(id, name, open, taskCategory, taskPriority, locationDetail);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }

        public Guid Id { get; }
        public bool Open { get; }
        public string Name { get; }
        public TaskCategory TaskCategory { get; set; }
        public TaskPriority TaskPriority { get; set; }
        public LocationDetail? LocationDetail { get; private set; }
    }
}
