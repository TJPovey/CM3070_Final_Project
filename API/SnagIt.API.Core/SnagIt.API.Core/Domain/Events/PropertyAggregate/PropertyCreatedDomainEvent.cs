using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.Shared;


namespace SnagIt.API.Core.Domain.Events.PropertyAggregate
{
    public class PropertyCreatedDomainEvent : INotification
    {
        private PropertyCreatedDomainEvent(SnagItProperty snagItProperty, UserId userId)
        {
            SnagItProperty = snagItProperty;
            UserId = userId;
        }

        public static PropertyCreatedDomainEvent Create(SnagItProperty snagItProperty, UserId userId)
            => new PropertyCreatedDomainEvent(snagItProperty, userId);

        public SnagItProperty SnagItProperty { get; }

        public UserId UserId { get; }
    }
}
