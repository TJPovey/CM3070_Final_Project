using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class PropertyId : ValueObject
    {
        [JsonConstructor]
        private PropertyId(Guid id, string name)
        {
            Id = !id.Equals(default) ? id : throw new DomainException($"A value for {nameof(id)} was not supplied.");
            Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new DomainException($"A value for {nameof(name)} was not supplied.");
        }

        public static PropertyId Create(Guid id, string name)
            => new PropertyId(id, name);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
