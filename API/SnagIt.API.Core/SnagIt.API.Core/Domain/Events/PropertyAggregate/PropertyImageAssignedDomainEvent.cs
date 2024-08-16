using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Property;


namespace SnagIt.API.Core.Domain.Events.PropertyAggregate
{
    public class PropertyImageAssignedDomainEvent : INotification
    {
        private PropertyImageAssignedDomainEvent(SnagItProperty snagItProperty)
        {
            SnagItProperty = snagItProperty;
        }

        public static PropertyImageAssignedDomainEvent Create(SnagItProperty snagItProperty)
            => new PropertyImageAssignedDomainEvent(snagItProperty);

        public SnagItProperty SnagItProperty { get; }
    }
}
