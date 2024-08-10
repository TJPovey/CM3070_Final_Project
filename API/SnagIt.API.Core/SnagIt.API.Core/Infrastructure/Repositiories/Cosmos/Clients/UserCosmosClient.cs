using System.Net;
using Microsoft.Azure.Cosmos;


namespace SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients
{
    public interface IUserCosmosClient : IBaseCosmosClient
    {
        Container GetSnagItUserContainer(Guid userId);

        Task CreateUserContainerIfNotExists(string containerId);

        Task Create<T>(Guid userId, T data, string partitionKey, CancellationToken cancellationToken = default)
            where T : class;

        Task Replace<T>(Guid userId, T data, string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class;

        Task<T> Get<T>(Guid userId, string id, string partitionKey,  CancellationToken cancellationToken = default)
            where T : class;

        Task<List<T>> Get<T>(Guid userId, QueryDefinition queryDefinition,CancellationToken cancellationToken = default)
            where T : class;
    }

    public class UserCosmosClient : BaseCosmosClient, IUserCosmosClient
    {
        private readonly CosmosClient _cosmosClient;

        public UserCosmosClient(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        }

        public Container GetSnagItUserContainer(Guid userId)
        {
            var container = _cosmosClient.GetContainer(
                CosmosConstants.SharedDatabaseId,
                userId.ToString());

            return container;
        }


        private ThroughputProperties GetDefaultThroughputProperties(int maxThroughput = 1000)
        {
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxThroughput);
            return throughputProperties;
        }

        private ContainerProperties GenerateContainerProperties(string containerId, string partitionKeyPath)
            => new ContainerProperties(containerId, partitionKeyPath);


        public async Task CreateUserContainerIfNotExists(string userId)
        {
            var targetDatabase = _cosmosClient.GetDatabase(CosmosConstants.SharedDatabaseId);

            var response = await targetDatabase.CreateContainerIfNotExistsAsync(
                GenerateContainerProperties(userId, CosmosConstants.PartitionKey),
                GetDefaultThroughputProperties());

            if (response.StatusCode.Equals(HttpStatusCode.Created))
            {
                return;
            }

            throw new Exception($"Error creating container: {userId} in database: {CosmosConstants.SharedDatabaseId}");
        }

        public async Task Create<T>(Guid userId, T data, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetSnagItUserContainer(userId);

            await CreateItem<T>(container, data, partitionKey, cancellationToken);
        }

        public async Task Replace<T>(Guid userId, T data, string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetSnagItUserContainer(userId);

            await ReplaceItem<T>(container, data, id, partitionKey, cancellationToken);
        }

        public async Task<T> Get<T>(Guid userId, string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetSnagItUserContainer(userId);

            return await GetItem<T>(container, id, partitionKey, cancellationToken);
        }

        public async Task<List<T>> Get<T>(Guid userId, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetSnagItUserContainer(userId);

            return await GetItems<T>(container, queryDefinition, cancellationToken);
        }
    }
}
