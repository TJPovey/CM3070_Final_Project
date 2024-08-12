using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class TaskCategory : Enumeration
    {
        public static TaskCategory OK = new TaskCategory(Guid.Parse("eecde81b-1d28-4d02-b372-b9ebadb48219"), nameof(OK).ToUpperInvariant());

        public static TaskCategory OBS = new TaskCategory(Guid.Parse("d44b93ed-b945-4cf8-b150-d6a7fd89dbe0"), nameof(OBS).ToUpperInvariant());

        public static TaskCategory CAT3 = new TaskCategory(Guid.Parse("19f04950-3d5b-4fb6-ace6-7e7d4e7fc3f3"), nameof(CAT3).ToUpperInvariant());

        public static TaskCategory CAT2 = new TaskCategory(Guid.Parse("248cf5c9-9629-4bc6-b90b-b7de6374e985"), nameof(CAT2).ToUpperInvariant());

        public static TaskCategory CAT1 = new TaskCategory(Guid.Parse("deaeee80-474b-43a0-9a3a-3c6107643fb1"), nameof(CAT1).ToUpperInvariant());

        [JsonConstructor]
        private TaskCategory(Guid id, string name)
            : base(id, name) { }

        public static IEnumerable<TaskCategory> List() => new[] { OK, OBS, CAT3, CAT2, CAT1 };

        public static TaskCategory FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state == null)
            {
                throw new DomainException($"Invalid {nameof(name)} provided for {nameof(TaskCategory)}. Available values for {nameof(UserRole)}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TaskCategory From(Guid id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null)
            {
                throw new DomainException($"Invalid {nameof(id)} provided for {nameof(TaskCategory)}. Available values for {nameof(UserRole)}: {string.Join(",", List().Select(s => $"{s.Id} ({s.Name})"))}");
            }

            return state;
        }
    }
}
