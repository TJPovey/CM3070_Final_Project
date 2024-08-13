using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class PropertyDetail : ValueObject
    {
        [JsonConstructor]
        private PropertyDetail(
            string propertyName, 
            string reportTitle,
            Uri? imageUri,
            LocationDetail locationDetail)
        {
            PropertyName = propertyName;
            ReportTitle = reportTitle;
            LocationDetail = locationDetail;
            ImageUri = imageUri;
        }

        public static PropertyDetail Create(
            string propertyName, 
            string reportTitle,
            Uri? imageUri,
            LocationDetail locationDetail)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new DomainException($"A value for {nameof(propertyName)} was not supplied.");
            }

            if (string.IsNullOrWhiteSpace(reportTitle))
            {
                throw new DomainException($"A value for {nameof(reportTitle)} was not supplied.");
            }

            if (locationDetail is null)
            {
                throw new DomainException($"A value for {nameof(locationDetail)} was not supplied.");
            }

            return new PropertyDetail(propertyName, reportTitle, imageUri, locationDetail);
        }

        public void UpdateImageUri(Uri imagePath)
        {
            ImageUri = imagePath;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PropertyName;
            yield return ReportTitle;
            yield return LocationDetail;
        }

        public Uri? ImageUri { get; private set; }
        public string PropertyName { get; }
        public string ReportTitle { get; }
        public LocationDetail LocationDetail { get; }
    }
}
