using MediatR;
using Microsoft.Azure.Cosmos;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Domain.SeedWork;
using SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients;
using System.Net;


namespace SnagIt.API.Core.Infrastructure.Repositiories
{
    public interface IUserRepository : IRepository<SnagItUser>
    {
        Task Add(SnagItUser user, CancellationToken cancellationToken = default);

        Task Update(SnagItUser user, CancellationToken cancellationToken = default);

        Task<SnagItUser> Get(Guid id, string username, CancellationToken cancellationToken = default);

        Task<List<SnagItUser>> GetAll(string username, CancellationToken cancellationToken = default);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMediator _mediator;
        private readonly IManagementCosmosClient _cosmosClient;

        public UserRepository(IMediator mediator, IManagementCosmosClient cosmosClient)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        }

        public async Task Add(SnagItUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // NEED TO SOMEHOW STORE HASH....

            var container = _cosmosClient.GetManagementContainer();
            using (var stream = _cosmosClient.ToStream(user))
            {
                using (var responseMessage = await container.CreateItemStreamAsync(
                    stream,
                    new PartitionKey(user.PartitionKey),
                    requestOptions: null,
                    cancellationToken))
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
            }

            await PublishDomainEvents(user: user);
        }

        public async Task Update(SnagItUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var container = _cosmosClient.GetManagementContainer();
            using (var stream = _cosmosClient.ToStream(user))
            {
                using (var responseMessage = await container.ReplaceItemStreamAsync(
                    stream,
                    user.Id.ToString(),
                    new PartitionKey(user.PartitionKey),
                    requestOptions: null,
                    cancellationToken))
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
            }

            await PublishDomainEvents(user);
        }

        public async Task<SnagItUser> Get(Guid id, string username, CancellationToken cancellationToken)
        {
            if (id.Equals(default))
            {
                throw new ArgumentException($"The value provided for {nameof(id)} is a default value.");
            }

            var container = _cosmosClient.GetManagementContainer();
            using (var responseMessage = await container.ReadItemStreamAsync(
                id.ToString(),
                new PartitionKey(SnagItUser.GeneratePartitionKey(username)),
                requestOptions: null,
                cancellationToken))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var streamResponse = _cosmosClient.FromStream<SnagItUser>(responseMessage.Content);

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

        public async Task<List<SnagItUser>> GetAll(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"The value provided for {nameof(username)} is a default value.");
            }

            var partitionKey = SnagItUser.GeneratePartitionKey(username);

            var query = new QueryDefinition("SELECT * FROM c WHERE c.PartitionKey=@partitionKey")
                .WithParameter("@partitionKey", partitionKey);

            return await _cosmosClient.Get<SnagItUser>(query, cancellationToken);
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
