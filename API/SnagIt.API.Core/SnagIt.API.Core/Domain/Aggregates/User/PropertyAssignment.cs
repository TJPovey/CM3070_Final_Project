using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.User
{
    public class PropertyAssignment : ValueObject
    {
        [JsonConstructor]
        private PropertyAssignment(PropertyId property, UserRole role)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public static PropertyAssignment Create(PropertyId property, UserRole role)
            => new PropertyAssignment(property, role);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Property;
            yield return Role;
        }

        public PropertyId Property { get; }

        public UserRole Role { get; }
    }
}
