using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;

namespace SnagIt.API.Core.Domain.Aggregates.Shared
{
    public class PropertyDetail : ValueObject
    {
        [JsonConstructor]
        private PropertyDetail(string propertyName, string reportTitle)
        {
            PropertyName = propertyName;
            ReportTitle = reportTitle;
        }

        public static PropertyDetail Create(string propertyName, string reportTitle)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new DomainException($"A value for {nameof(propertyName)} was not supplied.");
            }

            if (string.IsNullOrWhiteSpace(reportTitle))
            {
                throw new DomainException($"A value for {nameof(reportTitle)} was not supplied.");
            }

            return new PropertyDetail(propertyName, reportTitle);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PropertyName;
            yield return ReportTitle;
        }

        public string PropertyName { get; }
        public string ReportTitle { get; }
    }
}
