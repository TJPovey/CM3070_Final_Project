using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class PropertyId : ValueObject
    {
        [JsonConstructor]
        private PropertyId(Guid id, Guid ownerId, string name)
        {
            Id = !id.Equals(default) ? id : throw new DomainException($"A value for {nameof(id)} was not supplied.");
            OwnerId = !ownerId.Equals(default) ? ownerId : throw new DomainException($"A value for {nameof(ownerId)} was not supplied.");
            Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new DomainException($"A value for {nameof(name)} was not supplied.");
        }

        public static PropertyId Create(Guid id, Guid ownerId, string name)
            => new PropertyId(id, ownerId, name);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return OwnerId;
            yield return Name;
        }

        public Guid Id { get; }
        public Guid OwnerId { get; }
        public string Name { get; }
    }
}
