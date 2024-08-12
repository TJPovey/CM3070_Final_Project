using NodaTime;
using SnagIt.API.Core.Application.Features.Shared.Models;
using SnagIt.API.Core.Application.Models.Shared;


namespace SnagIt.API.Core.Application.Models.Task
{
    public class TaskDto : Response
    {
        public override ResponseData Data { get; set; }

        public class TaskDetailItem : ResponseItem<TaskDetail>
        {
            public override TaskDetail Item { get; set; }
        }

        public class TaskDetail
        {
            public string Id { get; set; }
            public string PropertyId { get; set; }
            public string Title { get; set; }
            public string Area { get; set; }
            public string Description { get; set; }
            public Instant DueDate { get; set; }
            public double EstimatedCost { get; set; }
            public Category Category { get; set; }
            public Priority Priority { get; set; }
            public UserAssignment AssignedUser { get; set; }
            public LocationGetDto? Location { get; set; }
        }

        public class Priority
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class Category
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class UserAssignment
        {
            public string Id { get; set; }
            public string UserName { get; set; }
        }
    }
}
