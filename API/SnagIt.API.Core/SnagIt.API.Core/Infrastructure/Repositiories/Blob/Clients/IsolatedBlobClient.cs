using Azure.Storage.Blobs;


namespace SnagIt.API.Core.Infrastructure.Repositiories.Blob.Clients
{
    public interface IIsolatedBlobClient
    {
        Task CreateContainerIfNotExists(Guid containerId);
    }

    public class IsolatedBlobClient : IIsolatedBlobClient
    {
        private readonly BlobServiceClient _blobClient;

        public IsolatedBlobClient()
        {
            var connectionString = Environment.GetEnvironmentVariable("BlobStorageConnecionString");
            _blobClient = new BlobServiceClient(connectionString);
        }

        public async Task CreateContainerIfNotExists(Guid containerId)
        {
            if (containerId.Equals(default))
            {
                throw new ArgumentException($"A value for {nameof(containerId)} was not supplied.", nameof(containerId));
            }

            var container = _blobClient.GetBlobContainerClient(containerId.ToString());
            if (!container.Exists())
            {
                var createResponse = await container.CreateAsync();
            }
        }
    }
}
