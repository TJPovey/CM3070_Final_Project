using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.User
{
    public class PropertyAssignment : ValueObject
    {
        [JsonConstructor]
        private PropertyAssignment(
            PropertyId property, 
            UserRole role, 
            LocationDetail locationDetail,
            string? imageUri)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            LocationDetail = locationDetail ?? throw new ArgumentNullException(nameof(locationDetail));
            ImageUri = imageUri;
        }

        public static PropertyAssignment Create(
            PropertyId property, 
            UserRole role, 
            LocationDetail locationDetail,
            string? imageUri)
            => new PropertyAssignment(property, role, locationDetail, imageUri);

        public void UpdateImageUri(string? uri)
        {
            ImageUri = uri;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Property;
            yield return Role;
            yield return LocationDetail;
        }

        public PropertyId Property { get; }

        public string? ImageUri { get; private set; }

        public UserRole Role { get; }

        public LocationDetail LocationDetail { get; }
    }
}
