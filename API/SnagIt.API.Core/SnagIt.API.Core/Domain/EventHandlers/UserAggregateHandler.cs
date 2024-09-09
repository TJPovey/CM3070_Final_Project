using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.Events.PropertyAggregate;
using SnagIt.API.Core.Domain.Events.UserAggregate;
using SnagIt.API.Core.Infrastructure.Repositiories;
using SnagIt.API.Core.Infrastructure.Repositiories.Blob.Clients;


namespace SnagIt.API.Core.Domain.EventHandlers
{
    public class UserAggregateHandler : 
        INotificationHandler<PropertyUserAssignedDomainEvent>,
        INotificationHandler<PropertyCreatedDomainEvent>,
        INotificationHandler<UserCreatedDomainEvent>,
        INotificationHandler<PropertyImageAssignedDomainEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIsolatedBlobClient _isolatedBlobClient;

        public UserAggregateHandler(IUserRepository userRepository, IIsolatedBlobClient isolatedBlobClient)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _isolatedBlobClient = isolatedBlobClient ?? throw new ArgumentNullException(nameof(isolatedBlobClient));
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(UserCreatedDomainEvent)} instance for {nameof(notification)} was not supplied.");
            }

            await _isolatedBlobClient.CreateContainerIfNotExists(notification.SnagItUser.Id);
        }

        public async Task Handle(PropertyCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(PropertyCreatedDomainEvent)} instance for {nameof(notification)} was not supplied.");
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
                notification.SnagItProperty.OwnerId.Id,
                notification.SnagItProperty.PropertyDetail.PropertyName);

            var locationDetail = LocationDetail.FromDegrees(
                notification.SnagItProperty.PropertyDetail.LocationDetail.Latitude,
                notification.SnagItProperty.PropertyDetail.LocationDetail.Longitude,
                notification.SnagItProperty.PropertyDetail.LocationDetail.Elevation);

            var imageUri = notification.SnagItProperty.PropertyDetail.ImageUri;

            // Since this user created this property, it must be true that they are also the owner.
            targetUser.AssignProperty(property, locationDetail, UserRole.Owner, imageUri);

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
                notification.SnagItProperty.OwnerId.Id,
                notification.SnagItProperty.PropertyDetail.PropertyName);

            var locationDetail = LocationDetail.FromDegrees(
                notification.SnagItProperty.PropertyDetail.LocationDetail.Latitude,
                notification.SnagItProperty.PropertyDetail.LocationDetail.Longitude,
                notification.SnagItProperty.PropertyDetail.LocationDetail.Elevation);

            var imageUri = notification.SnagItProperty.PropertyDetail.ImageUri;

            targetUser.AssignProperty(property, locationDetail, notification.UserAssignment.Role, imageUri);

            await _userRepository.UpdateUser(targetUser, cancellationToken);
        }

        public async Task Handle(PropertyImageAssignedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(PropertyUserAssignedDomainEvent)} instance for {nameof(notification)} was not supplied.");
            }

            var tasks = notification.SnagItProperty.AssignedUsers.Select(async user =>
                {
                    var targetUser = await _userRepository.GetUser(
                        user.UserId.Id,
                        user.UserId.Username,
                        cancellationToken);

                    var property = notification.SnagItProperty.Id;
                    targetUser.UpdatePropertyImageUri(
                        notification.SnagItProperty.Id, 
                        notification.SnagItProperty.PropertyDetail.ImageUri);

                    await _userRepository.UpdateUser(targetUser, cancellationToken);
                }
            ).ToList();

            await Task.WhenAll(tasks);
        }
    }
}
