using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class TaskPriority : Enumeration
    {
        public static TaskPriority Low = new TaskPriority(Guid.Parse("be88946d-e19d-45e2-a6e6-6c5a28052318"), nameof(Low).ToUpperInvariant());

        public static TaskPriority Medium = new TaskPriority(Guid.Parse("ac6c8c54-5b4c-4938-8190-be87a53717e3"), nameof(Medium).ToUpperInvariant());

        public static TaskPriority High = new TaskPriority(Guid.Parse("094ece08-9cfb-415a-8d7a-1133588d738f"), nameof(High).ToUpperInvariant());


        [JsonConstructor]
        private TaskPriority(Guid id, string name)
            : base(id, name) { }

        public static IEnumerable<TaskPriority> List() => new[] { Low, Medium, High };

        public static TaskPriority FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state == null)
            {
                throw new DomainException($"Invalid {nameof(name)} provided for {nameof(TaskPriority)}. Available values for {nameof(UserRole)}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TaskPriority From(Guid id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null)
            {
                throw new DomainException($"Invalid {nameof(id)} provided for {nameof(TaskPriority)}. Available values for {nameof(UserRole)}: {string.Join(",", List().Select(s => $"{s.Id} ({s.Name})"))}");
            }

            return state;
        }
    }
}
