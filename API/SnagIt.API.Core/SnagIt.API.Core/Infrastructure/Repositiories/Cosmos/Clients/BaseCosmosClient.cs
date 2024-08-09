using System.IO;
using System.Text;
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
    }
}
