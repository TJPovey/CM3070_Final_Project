using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class UserRole : Enumeration
    {
        public static UserRole Unknown = new UserRole(Guid.Parse("3a1ccd06-3342-46b3-8f3f-471710476f96"), nameof(Unknown).ToUpperInvariant());

        public static UserRole Unspecified = new UserRole(Guid.Parse("368857ca-98fd-453b-af62-c24e8e963fe5"), nameof(Unspecified).ToUpperInvariant());

        public static UserRole PlatformAdministrator = new UserRole(Guid.Parse("d2d80636-8910-42b1-828b-28443b9e489b"), "PlatformAdministrator");

        public static UserRole Owner = new UserRole(Guid.Parse("d64f869b-8978-4fdd-bd26-f3cd474d04aa"), "Owner");

        public static UserRole Contributer = new UserRole(Guid.Parse("98328385-13f9-404c-86a4-9e652cd0d693"), "Contributer");

        public static UserRole Reader = new UserRole(Guid.Parse("d773dd1a-745f-49f8-8f19-aac2c7f86744"), "Reader");

        [JsonConstructor]
        private UserRole(Guid id, string name)
            : base(id, name) { }

        public static IEnumerable<UserRole> List() => new[] { Unknown, Unspecified, PlatformAdministrator, Owner, Contributer, Reader };

        public static UserRole FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state == null)
            {
                throw new DomainException($"Invalid {nameof(name)} provided for {nameof(UserRole)}. Available values for {nameof(UserRole)}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static UserRole From(Guid id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null)
            {
                throw new DomainException($"Invalid {nameof(id)} provided for {nameof(UserRole)}. Available values for {nameof(UserRole)}: {string.Join(",", List().Select(s => $"{s.Id} ({s.Name})"))}");
            }

            return state;
        }
    }
}
