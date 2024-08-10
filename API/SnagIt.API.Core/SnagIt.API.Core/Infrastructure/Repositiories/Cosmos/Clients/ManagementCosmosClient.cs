using System.Net;
using Microsoft.Azure.Cosmos;


namespace SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients
{
    public interface IManagementCosmosClient : IBaseCosmosClient
    {
        Container GetManagementContainer();

        Container GetSnagItUserContainer(Guid userId);

        Task Create<T>(T data, string partitionKey, CancellationToken cancellationToken = default)
            where T : class;

        Task Replace<T>(T data, string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class;

        Task<T> Get<T>(string id, string partitionKey,  CancellationToken cancellationToken = default)
            where T : class;

        Task<List<T>> Get<T>(QueryDefinition queryDefinition,CancellationToken cancellationToken = default)
            where T : class;
    }

    public class ManagementCosmosClient : BaseCosmosClient, IManagementCosmosClient
    {
        private readonly CosmosClient _cosmosClient;

        public ManagementCosmosClient(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        }

        public Container GetManagementContainer()
        {
            var container = _cosmosClient.GetContainer(CosmosConstants.SharedDatabaseId, CosmosConstants.ManagementContainerId);

            return container;
        }

        public Container GetSnagItUserContainer(Guid userId)
        {
            var container = _cosmosClient.GetContainer(
                CosmosConstants.SharedDatabaseId,
                userId.ToString());

            return container;
        }


        public async Task Create<T>(T data, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            await CreateItem<T>(container, data, partitionKey, cancellationToken);
        }

        public async Task Replace<T>(T data, string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            await ReplaceItem<T>(container, data, id, partitionKey, cancellationToken);
        }

        public async Task<T> Get<T>(string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            return await GetItem<T>(container, id, partitionKey, cancellationToken);
        }

        public async Task<List<T>> Get<T>(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            return await GetItems<T>(container, queryDefinition, cancellationToken);
        }
    }
}
