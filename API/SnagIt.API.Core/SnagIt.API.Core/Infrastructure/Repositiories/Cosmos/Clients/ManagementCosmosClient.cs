using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Cosmos;

namespace SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients
{
    public interface IManagementCosmosClient : IBaseCosmosClient
    {
        Container GetManagementContainer();

        Task CreateContainerIfNotExists(string containerId);

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


        private ThroughputProperties GetDefaultThroughputProperties(int maxThroughput = 1000)
        {
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxThroughput);
            return throughputProperties;
        }

        private ContainerProperties GenerateContainerProperties(string containerId, string partitionKeyPath)
            => new ContainerProperties(containerId, partitionKeyPath);


        public async Task CreateContainerIfNotExists(string containerId)
        {
            var targetDatabase = _cosmosClient.GetDatabase(CosmosConstants.SharedDatabaseId);

            var response = await targetDatabase.CreateContainerIfNotExistsAsync(
                GenerateContainerProperties(containerId, CosmosConstants.PartitionKey),
                GetDefaultThroughputProperties());

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                return;
            }

            throw new Exception($"Error creating container: {containerId} in database: {CosmosConstants.SharedDatabaseId}");
        }

        public async Task Create<T>(T data, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            using (var stream = ToStream(data))
            {
                using (var responseMessage = await container.CreateItemStreamAsync(
                    stream,
                    new PartitionKey(partitionKey),
                    requestOptions: null,
                    cancellationToken))
                {

                    responseMessage.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task Replace<T>(T data, string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            using (var stream = ToStream(data))
            {
                using (var responseMessage = await container.ReplaceItemStreamAsync(
                    stream,
                    id,
                    new PartitionKey(partitionKey),
                    requestOptions: null,
                    cancellationToken))
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task<T> Get<T>(string id, string partitionKey, CancellationToken cancellationToken = default)
            where T : class
        {
            var container = GetManagementContainer();

            using (var responseMessage = await container.ReadItemStreamAsync(
                id: id,
                new PartitionKey(partitionKey),
                requestOptions: null,
                cancellationToken))
            {
                var requestCharge = responseMessage.Headers.RequestCharge;

                if (responseMessage.IsSuccessStatusCode)
                {
                    var streamResponse = FromStream<T>(responseMessage.Content);

                    return streamResponse;
                }

                if (responseMessage.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    return null;
                }

                responseMessage.EnsureSuccessStatusCode();

                return default;
            }
        }

        public async Task<List<T>> Get<T>(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
            where T : class
        {
            var itemList = new List<T>();

            var container = GetManagementContainer();

            var feedIterator = container.GetItemQueryIterator<T>(queryDefinition, null, null);

            while (feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync(cancellationToken))
                {
                    itemList.Add(item);
                }
            }

            return itemList;
        }
    }
}
