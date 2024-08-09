using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class UserDetail : ValueObject
    {
        [Obsolete("This only exists for JSON deserialisation. Do not use it for any other purpose.")]
        [JsonConstructor]
        private UserDetail(string firstName, string lastName, string fullName, string userName, string email)
        {
            FullName = firstName;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
        }

        private UserDetail(string firstName, string lastName, string userName, string email)
        {
            FirstName = !string.IsNullOrWhiteSpace(firstName) ? firstName.Trim() : throw new DomainException($"A value for {nameof(firstName)} was not supplied.");
            LastName = !string.IsNullOrWhiteSpace(lastName) ? lastName.Trim() : throw new DomainException($"A value for {nameof(lastName)} was not supplied.");
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName.Trim() : throw new DomainException($"A value for {nameof(userName)} was not supplied.");
            Email = !string.IsNullOrWhiteSpace(email) ? email.Trim() : throw new DomainException($"A value for {nameof(email)} was not supplied.");
            FullName = $"{LastName}, {FirstName}";
        }

        public static UserDetail Create(string firstName, string lastName, string userName, string email)
        {
            return new UserDetail(firstName, lastName, userName, email);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return FirstName;
            yield return LastName;
            yield return UserName;
            yield return Email;
        }

        public string FullName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string UserName { get; }
        public string Email { get; }
    }
}
