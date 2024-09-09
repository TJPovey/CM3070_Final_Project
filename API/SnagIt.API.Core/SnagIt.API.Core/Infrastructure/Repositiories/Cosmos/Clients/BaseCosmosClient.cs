using System.IO;
using System.Net;
using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace SnagIt.API.Core.Infrastructure.Repositiories.Cosmos.Clients
{
    public interface IBaseCosmosClient
    {
        T FromStream<T>(Stream stream);

        Stream ToStream<T>(T input);
    }

    public class BaseCosmosClient : IBaseCosmosClient
    {
        private JsonSerializerSettings _serializerSettings;
        private JsonSerializer _serializer;

        public BaseCosmosClient()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings = serializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            _serializerSettings = serializerSettings;
            _serializer = JsonSerializer.Create(serializerSettings);
        }

        public T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)(stream);
                }

                using (StreamReader sr = new StreamReader(stream))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                    {
                        return _serializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }

        public Stream ToStream<T>(T input)
        {
            MemoryStream streamPayload = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(streamPayload, encoding: Encoding.Default, bufferSize: 1024, leaveOpen: true))
            {
                using (JsonWriter writer = new JsonTextWriter(streamWriter))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.None;
                    _serializer.Serialize(writer, input);
                    writer.Flush();
                    streamWriter.Flush();
                }
            }

            streamPayload.Position = 0;
            return streamPayload;
        }

        public async Task CreateItem<T>(
            Container container, 
            T data, 
            string partitionKey, 
            CancellationToken cancellationToken = default)
            where T : class
        {
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

        public async Task ReplaceItem<T>(
            Container container, 
            T data, 
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default)
            where T : class
        {
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

        public async Task<T> GetItem<T>(
            Container container, 
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default)
            where T : class
        {
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

        public async Task<List<T>> GetItems<T>(
            Container container, 
            QueryDefinition queryDefinition, 
            CancellationToken cancellationToken = default)
            where T : class
        {
            var itemList = new List<T>();

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
