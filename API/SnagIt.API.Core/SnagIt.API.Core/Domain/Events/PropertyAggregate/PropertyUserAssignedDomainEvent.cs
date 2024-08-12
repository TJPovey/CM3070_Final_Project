using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.User;


namespace SnagIt.API.Core.Domain.Events.PropertyAggregate
{
    public class PropertyUserAssignedDomainEvent : INotification
    {
        private PropertyUserAssignedDomainEvent(SnagItProperty snagItProperty, UserAssignment userAssignment)
        {
            SnagItProperty = snagItProperty;
            UserAssignment = userAssignment;
        }

        public static PropertyUserAssignedDomainEvent Create(SnagItProperty snagItProperty, UserAssignment userAssignment)
            => new PropertyUserAssignedDomainEvent(snagItProperty, userAssignment);

        public SnagItProperty SnagItProperty { get; }

        public UserAssignment UserAssignment { get; }
    }
}
