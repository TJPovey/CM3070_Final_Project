using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using SnagIt.API.Core.Domain.Events.PropertyAggregate;
using SnagIt.API.Core.Infrastructure.Repositiories;


namespace SnagIt.API.Core.Domain.EventHandlers
{
    public class PropertyAggregateHandler : 
        INotificationHandler<TaskStatusUpdatedDomainEvent>,
        INotificationHandler<TaskCreatedDomainEvent>
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyAggregateHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        }

        public async Task Handle(TaskCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(TaskCreatedDomainEvent)} instance for {nameof(notification)} was not supplied.");
            }

            var taskDetails = notification.SnagItTask.TaskDetail;
            var ownerId = notification.OwnerId.Id;
            var propertyId = taskDetails.Property.Id;

            var targetProperty = await _propertyRepository.GetProperty(
                propertyId,
                ownerId,
                cancellationToken);

            if (targetProperty is null)
            {
                throw new InvalidOperationException($"A {nameof(SnagItProperty)} entity could not be found to update.");
            }


            var taskId = TaskId.Create(
                notification.SnagItTask.Id,
                taskDetails.Title,
                taskDetails.Open,
                taskDetails.TaskCategory,
                taskDetails.TaskPriority,
                taskDetails.LocationDetail);

            targetProperty.AssignTaskToProperty(taskId);

            await _propertyRepository.UpdateProperty(targetProperty, ownerId, cancellationToken);
        }

        public async Task Handle(TaskStatusUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
            {
                throw new ArgumentException($"A {nameof(TaskStatusUpdatedDomainEvent)} instance for {nameof(notification)} was not supplied.");
            }

            var taskDetails = notification.SnagItTask.TaskDetail;
            var ownerId = notification.OwnerId.Id;
            var propertyId = taskDetails.Property.Id;

            var targetProperty = await _propertyRepository.GetProperty(
                propertyId,
                ownerId,
                cancellationToken);

            if (targetProperty is null)
            {
                throw new InvalidOperationException($"A {nameof(SnagItProperty)} entity could not be found to update.");
            }


            var taskId = TaskId.Create(
                notification.SnagItTask.Id,
                taskDetails.Title,
                taskDetails.Open,
                taskDetails.TaskCategory,
                taskDetails.TaskPriority,
                taskDetails.LocationDetail);

            targetProperty.UpdateTask(taskId);

            await _propertyRepository.UpdateProperty(targetProperty, ownerId, cancellationToken);
        }
    }
}
