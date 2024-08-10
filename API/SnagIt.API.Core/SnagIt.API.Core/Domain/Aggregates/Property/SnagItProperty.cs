using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.Events.PropertyAggregate;
using SnagIt.API.Core.Domain.Events.UserAggregate;
using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using System.Collections.ObjectModel;


namespace SnagIt.API.Core.Domain.Aggregates.Property
{
    public class SnagItProperty : Entity, IAggregateRoot
    {
        public static string PartitionKeyPrefix = "property-";
        private readonly List<UserAssignment> _assignedUsers;

        [Obsolete("This only exists for JSON deserialisation. Do not use it for any other purpose.")]
        [JsonConstructor]
        private SnagItProperty(
            Guid id,
            string partitionKey,
            string type,
            string passwordHash,
            PropertyDetail propertyDetail,
            List<UserAssignment> assignedUsers)
        {
            Id = id;
            PartitionKey = partitionKey;
            Type = type;
            PropertyDetail = propertyDetail;
            _assignedUsers = assignedUsers;
        }

        private SnagItProperty(Guid id, PropertyDetail propertyDetail, UserId userId)
        {
            Id = !id.Equals(default) ? id : throw new DomainException(nameof(id));
            PartitionKey = GeneratePartitionKey(Id);
            Type = nameof(SnagItProperty);
            PropertyDetail = propertyDetail ?? throw new DomainException(nameof(propertyDetail));
            _assignedUsers = new List<UserAssignment>()
            {
                UserAssignment.Create(userId, UserRole.Owner)
            };
        }

        public static SnagItProperty Create(PropertyDetail propertyDetail, UserId userId)
        {
            var property = new SnagItProperty(Guid.NewGuid(), propertyDetail, userId);

            var @event = PropertyCreatedDomainEvent.Create(property, userId);

            property.AddDomainEvent(@event);

            return property;
        }

        public static string GeneratePartitionKey(Guid propertyId)
            => $"{PartitionKeyPrefix}{propertyId}";

        private void AssignUserToPropertyWithRole(UserId userId, UserRole userRole)
        {
            if (userId is null)
            {
                throw new DomainException($"A {nameof(PropertyId)} instance for {nameof(userId)} was not supplied.");
            }

            if (userRole is null)
            {
                throw new DomainException($"A {nameof(UserRole)} instance for {nameof(userRole)} was not supplied.");
            }


            if (_assignedUsers.Any(x => x.UserId.Equals(userId) && x.Role.Equals(userRole)))
            {
                //  User has already been assigned to target property with same role.
                return;
            }

            // Remove existing assignment to chage the user role
            var existingUserAssignment = _assignedUsers.FirstOrDefault(x => x.UserId.Equals(userId));
            if (existingUserAssignment != null)
            {
                _assignedUsers.Remove(existingUserAssignment);
            }

            var userAssignment = UserAssignment.Create(userId, userRole);

            _assignedUsers.Add(userAssignment);

            var @event = PropertyUserAssignedDomainEvent.Create(
                    this,
                    userAssignment);

            AddDomainEvent(@event);
        }

        public void RemoveFromProperty(PropertyId property)
        {
            //
        }

        public PropertyDetail PropertyDetail { get; private set; }

        public ReadOnlyCollection<UserAssignment> AssignedUsers => _assignedUsers.AsReadOnly();
    }
}
