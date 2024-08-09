
namespace SnagIt.API.Core.Application.Features.Shared.Models
{
    public partial class ResponseError
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public int Status { get; set; }

        public string Detail { get; set; }

        public string Instance { get; set; }
    }
}
