using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.Shared;


namespace SnagIt.API.Core.Application.Models.Property
{
    public class PropertyDto : Response
    {
        public override ResponseData Data { get; set; }

        public class PropertyDetailItem : ResponseItem<PropertyDetail>
        {
            public override PropertyDetail Item { get; set; }
        }

        public class PropertyDetail
        {
            public string Id { get; set; }
            public string PropertyName { get; set; }
            public string ReportTitle { get; set; }
            public Uri? ImageUri { get; set; }
            public Uri? WriteToken { get; set; }
            public LocationGetDto Location { get; set; }
            public UserRole OwnerId { get; set; }
            public List<UserAssignment> UserAssignments { get; set; }
            public List<TaskAssignment> TaskAssignments { get; set; }
        }

        public class TaskAssignment
        {
            public string Id { get; set; }
            public bool Open { get; set; }
            public string Name { get; set; }
            public TaskCategory TaskCategory { get; set; }
            public TaskPriority TaskPriority { get; set; }
            public LocationGetDto? Location { get; set; }
        }

        public class TaskCategory
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class TaskPriority
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class UserAssignment
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public UserRole Role { get; set; }
        }

        public class UserRole
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
