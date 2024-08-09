using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Events.UserAggregate;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;


namespace SnagIt.API.Core.Domain.Aggregates.User
{
    public sealed class SnagItUser : Entity, IAggregateRoot
    {
        public static string PartitionKeyPrefix = "user-";

        private List<PropertyAssignment> _assignedProperties;

        [Obsolete("This only exists for JSON deserialisation. Do not use it for any other purpose.")]
        [JsonConstructor]
        private SnagItUser(
            Guid id,
            string partitionKey,
            string type,
            string passwordHash,
            UserDetail userDetail,
            List<PropertyAssignment> assignedProperties)
        {
            Id = id;
            PartitionKey = partitionKey;
            Type = type;
            UserDetail = userDetail;
            PasswordHash = passwordHash;
            _assignedProperties = assignedProperties;
        }

        private SnagItUser(Guid id, UserDetail userDetail)
        {
            Id = !id.Equals(default) ? id : throw new DomainException(nameof(id));
            PartitionKey = GeneratePartitionKey(userDetail.UserName);
            Type = nameof(SnagItUser);
            UserDetail = userDetail ?? throw new DomainException(nameof(userDetail));
            _assignedProperties = new List<PropertyAssignment>();
        }

        public static SnagItUser Create(Guid id, string password, UserDetail userDetail)
        {
            var user = new SnagItUser(id, userDetail);

            var passwordHasher = new PasswordHasher<SnagItUser>();
            var passwordHash = passwordHasher.HashPassword(user, password);
            user.PasswordHash = passwordHash;

            var @event = UserCreatedDomainEvent.Create(user);

            user.AddDomainEvent(@event);

            return user;
        }

        public static string GeneratePartitionKey(string username)
            => $"{PartitionKeyPrefix}{username}";


        private void AssignToPropertyWithRole(PropertyId propertyId, UserRole userRole)
        {
            if (propertyId is null)
            {
                throw new DomainException($"A {nameof(PropertyId)} instance for {nameof(propertyId)} was not supplied.");
            }

            if (userRole is null)
            {
                throw new DomainException($"A {nameof(UserRole)} instance for {nameof(userRole)} was not supplied.");
            }


            if (_assignedProperties.Any(x => x.Property.Equals(propertyId) && x.Role.Equals(userRole)))
            {
                //  User has already been assigned to target property with same role.
                return;
            }

            var existinPropertyAssignment = _assignedProperties.FirstOrDefault(x => x.Property.Equals(propertyId));
            if (existinPropertyAssignment != null)
            {
                _assignedProperties.Remove(existinPropertyAssignment);
            }

            var propertyAssignment = PropertyAssignment.Create(propertyId, userRole);

            _assignedProperties.Add(propertyAssignment);

            var @event = UserPropertyAssignedDomainEvent.Create(
                    this,
                    propertyAssignment);

            AddDomainEvent(@event);
        }

        public void RemoveFromProperty(PropertyId property)
        {
            if (property is null)
            {
                throw new DomainException($"A {nameof(PropertyId)} instance for {nameof(property)} was not supplied.");
            }

            var existingOrganisationAssignment = _assignedProperties.FirstOrDefault(x => x.Property.Id.Equals(property.Id));
            if (existingOrganisationAssignment is null)
            {
                //  Property isn't assigned to target User.
                return;
            }

            _assignedProperties.Remove(existingOrganisationAssignment);

            //var @event = UserOrganisationRemovedDomainEvent.Create(
            //    UserOrganisationRemovedDomainEvent.OrganisationIdInternal.Create(existingOrganisationAssignment.Property.Id),
            //    UserOrganisationRemovedDomainEvent.UserIdInternal.Create(Id));

            //AddDomainEvent(@event);
        }

        public UserDetail UserDetail { get; private set; }

        public string PasswordHash { get; private set; }

        public IReadOnlyCollection<PropertyAssignment> AssignedProperties => _assignedProperties.AsReadOnly();
    }

}
