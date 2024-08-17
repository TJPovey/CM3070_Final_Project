using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System.IO;


namespace SnagIt.API.Core.Infrastructure.Repositiories.Blob.Clients
{
    public interface IIsolatedBlobClient
    {
        Task CreateContainerIfNotExists(Guid containerId);

        Task<Uri?> GetWriteToken(Guid containerId);

        string GetPropertyImageContainerPath(Guid containerId, Guid propertyGuid, string imageName);

        string GetTaskImageContainerPath(Guid containerId, Guid propertyGuid, Guid taskGuid, string imageName);

        Task<Uri?> GetReadToken(string path);
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
            if (!await container.ExistsAsync())
            {
                var createResponse = await container.CreateAsync();
            }
        }

        public async Task<Uri?> GetWriteToken(Guid containerId)
        {
            if (containerId.Equals(default))
            {
                throw new ArgumentException($"A value for {nameof(containerId)} was not supplied.", nameof(containerId));
            }

            var userContainer = _blobClient.GetBlobContainerClient(containerId.ToString());

            try
            {
                await userContainer.ExistsAsync();
            }
            catch (RequestFailedException)
            {
                // Folder does not yet exist
                // Possibly due to no images being assigned to the property yet
                return null;
            }

            return userContainer.GenerateSasUri(
                BlobContainerSasPermissions.Write |
                BlobContainerSasPermissions.Create |
                BlobContainerSasPermissions.Add |
                BlobContainerSasPermissions.Read,
                DateTimeOffset.Now.AddHours(1));
        }

        public async Task<Uri?> GetReadToken(string imagePath)
        {
            if (imagePath is null)
            {
                throw new ArgumentException($"A value for {nameof(imagePath)} was not supplied.", nameof(imagePath));
            }

            var container = _blobClient.GetBlobContainerClient(imagePath);

            try
            {
                await container.ExistsAsync();
            }
            catch (RequestFailedException)
            {
                // Blob does not yet exist
                // Possibly due to no images being assigned to the property yet
                return null;
            }

            return container.GenerateSasUri(
                BlobContainerSasPermissions.Read,
                DateTimeOffset.Now.AddHours(1));
        }

        public string GetPropertyImageContainerPath(
            Guid containerId,
            Guid propertyGuid,
            string imageName)
        {
            if (containerId.Equals(default))
            {
                throw new ArgumentException($"A value for {nameof(containerId)} was not supplied.", nameof(containerId));
            }

            var containerPath = Path.Combine(
                containerId.ToString(),
                propertyGuid.ToString(),
                BlobConstants.ImagesFolderName,
                imageName);

            return containerPath;
        }

        public string GetTaskImageContainerPath(
            Guid containerId,
            Guid propertyGuid,
            Guid taskGuid,
            string imageName)
        {
            if (containerId.Equals(default))
            {
                throw new ArgumentException($"A value for {nameof(containerId)} was not supplied.", nameof(containerId));
            }

            var containerPath = Path.Combine(
                containerId.ToString(),
                propertyGuid.ToString(),
                BlobConstants.TasksFolderName,
                taskGuid.ToString(),
                imageName);

            return containerPath;
        }
    }
}
