using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class UserId : ValueObject
    {
        [JsonConstructor]
        private UserId(Guid id, string username)
        {
            Id = !id.Equals(default) ? id : throw new DomainException($"A value for {nameof(id)} was not supplied.");
            Username = !string.IsNullOrWhiteSpace(username) ? username.Trim() : throw new DomainException($"A value for {nameof(username)} was not supplied.");
        }

        public static UserId Create(Guid id, string name)
            => new UserId(id, name);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }

        public Guid Id { get; }

        public string Username { get; }
    }
}
