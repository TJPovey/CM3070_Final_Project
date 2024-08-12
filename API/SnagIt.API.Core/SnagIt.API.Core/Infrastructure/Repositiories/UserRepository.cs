using MediatR;
using Microsoft.Azure.Cosmos;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.SeedWork;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients;


namespace SnagIt.API.Core.Infrastructure.Repositiories
{
    public interface IUserRepository : IRepository<SnagItUser>
    {
        Task AddUser(SnagItUser user, CancellationToken cancellationToken = default);

        Task UpdateUser(SnagItUser user, CancellationToken cancellationToken = default);

        Task<SnagItUser> GetUser(Guid id, string username, CancellationToken cancellationToken = default);

        Task<List<SnagItUser>> GetAllUsersWithUsername(string username, CancellationToken cancellationToken = default);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMediator _mediator;
        private readonly IManagementCosmosClient _managementCosmosClient;
        private readonly IUserCosmosClient _userCosmosClient;

        public UserRepository(
            IMediator mediator, 
            IManagementCosmosClient managementCosmosClient,
            IUserCosmosClient userCosmosClient)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _managementCosmosClient = managementCosmosClient ?? throw new ArgumentNullException(nameof(managementCosmosClient));
            _userCosmosClient = userCosmosClient ?? throw new ArgumentNullException(nameof(userCosmosClient));
        }

        public async Task AddUser(SnagItUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Create a new container for the current user
            await _userCosmosClient.CreateUserContainerIfNotExists(user.Id.ToString());

            // Also add the user to the management container
            await _managementCosmosClient.Create<SnagItUser>(
                user,
                user.PartitionKey,
                cancellationToken);

            await PublishDomainEvents(user);
        }

        public async Task UpdateUser(SnagItUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _managementCosmosClient.Replace(
                user, 
                user.Id.ToString(), 
                user.PartitionKey,
                cancellationToken);

            await PublishDomainEvents(user);
        }

        public async Task<SnagItUser> GetUser(Guid id, string username, CancellationToken cancellationToken)
        {
            if (id.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(id)} is a default value.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException($"The value provided for {nameof(username)} is empty.");
            }

            return await _managementCosmosClient.Get<SnagItUser>(
                id.ToString(), 
                SnagItUser.GeneratePartitionKey(username), 
                cancellationToken);
        }

        public async Task<List<SnagItUser>> GetAllUsersWithUsername(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"The value provided for {nameof(username)} is a default value.");
            }

            var partitionKey = SnagItUser.GeneratePartitionKey(username);

            var query = new QueryDefinition("SELECT * FROM c WHERE c.PartitionKey=@partitionKey")
                .WithParameter("@partitionKey", partitionKey);

            return await _managementCosmosClient.Get<SnagItUser>(query, cancellationToken);
        }

        private async Task PublishDomainEvents(Entity user)
        {
            if (user.DomainEvents != null)
            {
                foreach (var domainEvent in user.DomainEvents)
                {
                    await _mediator.Publish(domainEvent);
                }

                user.ClearDomainEvents();
            }
        }
    }
}
