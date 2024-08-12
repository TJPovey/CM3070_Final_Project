

using SnagIt.API.Core.Application.Models.Shared;

namespace SnagIt.API.Core.Application.Models.Property
{
    public class PropertyPostDto
    {
        public string PropertyName { get; set; }
        public string ReportTitle { get; set; }
        public LocationPostDto Location { get; set; }
    }
}
