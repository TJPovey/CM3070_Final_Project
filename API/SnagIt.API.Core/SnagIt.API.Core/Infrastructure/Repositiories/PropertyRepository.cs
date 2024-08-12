using MediatR;
using SnagIt.API.Core.Domain.Aggregates.Property;
using SnagIt.API.Core.Domain.Aggregates.SnagItTask;
using SnagIt.API.Core.Domain.SeedWork;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients;


namespace SnagIt.API.Core.Infrastructure.Repositiories
{
    public interface IPropertyRepository : IRepository<SnagItProperty>
    {
        Task AddProperty(SnagItProperty property, Guid userId, CancellationToken cancellationToken = default);

        Task UpdateProperty(SnagItProperty property, Guid userId, CancellationToken cancellationToken = default);

        Task<SnagItProperty> GetProperty(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task AddTask(SnagItTask task, Guid userId, CancellationToken cancellationToken = default);

        Task<SnagItTask> GetTask(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }

    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMediator _mediator;
        private readonly IUserCosmosClient _userCosmosClient;

        public PropertyRepository(IMediator mediator, IUserCosmosClient userCosmosClient)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userCosmosClient = userCosmosClient ?? throw new ArgumentNullException(nameof(userCosmosClient));
        }

        public async Task AddProperty(SnagItProperty property, Guid userId, CancellationToken cancellationToken = default)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (userId.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(userId)} is a default value.");
            }

            await _userCosmosClient.Create<SnagItProperty>(
                userId,
                property,
                property.PartitionKey,
                cancellationToken);


            await PublishDomainEvents(property);
        }

        public async Task<SnagItProperty> GetProperty(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            if (id.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(id)} is a default value.");
            }

            if (userId.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(userId)} is a default value.");
            }

            return await _userCosmosClient.Get<SnagItProperty>(
                userId,
                id.ToString(),
                SnagItProperty.GeneratePartitionKey(id),
                cancellationToken);
        }

        public async Task UpdateProperty(SnagItProperty property, Guid userId, CancellationToken cancellationToken = default)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (userId.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(userId)} is a default value.");
            }

            await _userCosmosClient.Replace(
                userId,
                property,
                property.Id.ToString(),
                property.PartitionKey,
                cancellationToken);

            await PublishDomainEvents(property);
        }

        public async Task AddTask(SnagItTask task, Guid userId, CancellationToken cancellationToken = default)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (userId.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(userId)} is a default value.");
            }

            await _userCosmosClient.Create<SnagItTask>(
                userId,
                task,
                task.PartitionKey,
                cancellationToken);


            await PublishDomainEvents(task);
        }

        public async Task<SnagItTask> GetTask(
            Guid id, 
            Guid userId, 
            CancellationToken cancellationToken = default)
        {
            if (id.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(id)} is a default value.");
            }

            if (userId.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(userId)} is a default value.");
            }

            return await _userCosmosClient.Get<SnagItTask>(
                userId,
                id.ToString(),
                SnagItTask.GeneratePartitionKey(id),
                cancellationToken);
        }

        private async Task PublishDomainEvents(Entity property)
        {
            if (property.DomainEvents != null)
            {
                foreach (var domainEvent in property.DomainEvents)
                {
                    await _mediator.Publish(domainEvent);
                }

                property.ClearDomainEvents();
            }
        }
    }
}
