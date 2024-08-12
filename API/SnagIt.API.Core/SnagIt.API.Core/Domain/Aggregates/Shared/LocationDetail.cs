using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class LocationDetail : ValueObject
    {
        [JsonConstructor]
        private LocationDetail(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static LocationDetail FromDegrees(decimal latitude, decimal longitude)
        {
            if (latitude < -90 || latitude > 90)
            {
                throw new DomainException($"The value provided for {nameof(latitude)} is out of bounds.");
            }

            if (longitude < -180 || longitude > 180)
            {
                throw new DomainException($"The value provided for {nameof(longitude)} is out of bounds.");
            }

            return new LocationDetail(latitude, longitude);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Latitude;
            yield return Longitude;
        }

        public decimal Latitude { get; }
        public decimal Longitude { get; }
    }
}
