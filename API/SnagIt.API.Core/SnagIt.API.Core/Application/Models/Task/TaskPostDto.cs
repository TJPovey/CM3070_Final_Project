using SnagIt.API.Core.Application.Models.Shared;

namespace SnagIt.API.Core.Application.Models.Task
{
    public class TaskPostDto
    {
        public Guid PropertyId { get; set; }
        public Guid UserPropertyOwnerId { get; set; }
        public string Title { get; set; }
        public string Area { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        public double EstimatedCost { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public Guid AssignedToUserId { get; set; }
        public string AssignedToUserUserName { get; set; }
        public LocationPostDto? Location {  get; set; }
    }
}
