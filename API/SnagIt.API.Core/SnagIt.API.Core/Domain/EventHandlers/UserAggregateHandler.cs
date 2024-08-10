using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.Events.PropertyAggregate;
using SnagIt.API.Core.Domain.Events.UserAggregate;
using SnagIt.API.Core.Infrastructure.Repositiories;


namespace SnagIt.API.Core.Domain.EventHandlers
{
    public class UserAggregateHandler : 
        INotificationHandler<PropertyUserAssignedDomainEvent>,
        INotificationHandler<PropertyCreatedDomainEvent>
    {
        private readonly IUserRepository _userRepository;

        public UserAggregateHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task Handle(PropertyCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(PropertyUserAssignedDomainEvent)} instance for {nameof(notification)} was not supplied.");
            }

            var targetUser = await _userRepository.GetUser(
                notification.UserId.Id,
                notification.UserId.Username,
                cancellationToken);

            if (targetUser is null)
            {
                throw new InvalidOperationException($"A {nameof(SnagItUser)} entity could not be found to update.");
            }

            var property = PropertyId.Create(
                notification.SnagItProperty.Id,
                notification.SnagItProperty.PropertyDetail.PropertyName);

            // Since this user created this property, it must be true that they are also the owner.
            targetUser.AssignProperty(property, UserRole.Owner);

            await _userRepository.UpdateUser(targetUser, cancellationToken);
        }


        public async Task Handle(PropertyUserAssignedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(PropertyUserAssignedDomainEvent)} instance for {nameof(notification)} was not supplied.");
            }

            var targetUser = await _userRepository.GetUser(
                notification.UserAssignment.UserId.Id,
                notification.UserAssignment.UserId.Username,
                cancellationToken);

            if (targetUser is null)
            {
                throw new InvalidOperationException($"A {nameof(SnagItUser)} entity could not be found to update.");
            }

            var property = PropertyId.Create(
                notification.SnagItProperty.Id,
                notification.SnagItProperty.PropertyDetail.PropertyName);


            targetUser.AssignProperty(property, notification.UserAssignment.Role);

            await _userRepository.UpdateUser(targetUser, cancellationToken);
        }
    }
}
