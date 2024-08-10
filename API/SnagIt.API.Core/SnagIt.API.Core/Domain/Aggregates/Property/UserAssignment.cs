using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.User
{
    public class UserAssignment : ValueObject
    {
        [JsonConstructor]
        private UserAssignment(UserId userId, UserRole role)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public static UserAssignment Create(UserId userId, UserRole role)
            => new UserAssignment(userId, role);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return UserId;
            yield return Role;
        }

        public UserId UserId { get; }

        public UserRole Role { get; }
    }
}
