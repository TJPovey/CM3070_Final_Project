using SnagIt.API.Core.Application.Models.Shared;
using SnagIt.API.Core.Domain.Aggregates.SnagItTask;
using static SnagIt.API.Core.Application.Models.Task.TaskDto;
using TaskDetail = SnagIt.API.Core.Application.Models.Task.TaskDto.TaskDetail;


namespace SnagIt.API.Core.Application.Extensions.Mapping.Tasks
{
    public static class SnagItTaskExtensions
    {
        public static TaskDetailItem ToTaskDetailItem(this SnagItTask task, Uri? imageUri)
        {
            var location = task.TaskDetail.LocationDetail is null ?
                null :
                new LocationGetDto
                {
                    Latitude = task.TaskDetail.LocationDetail.Latitude,
                    Longitude = task.TaskDetail.LocationDetail.Longitude,
                    Elevation = task.TaskDetail.LocationDetail.Elevation
                };


            return new TaskDetailItem
            {
                Kind = nameof(TaskDetailItem),
                Id = task.Id.ToString(),
                Item = new TaskDetail
                {
                    Id = task.Id.ToString(),
                    PropertyId = task.TaskDetail.Property.Id.ToString(),
                    Title = task.TaskDetail.Title,
                    Open = task.TaskDetail.Open,
                    Area = task.TaskDetail.Area,
                    Description = task.TaskDetail.Description,
                    DueDate = task.TaskDetail.DueDate,
                    EstimatedCost = task.TaskDetail.EstimatedCost,
                    ImageUri = imageUri,
                    Category = new Category
                    {
                        Id = task.TaskDetail.TaskCategory.Id.ToString(),
                        Name = task.TaskDetail.TaskCategory.Name
                    },
                    Priority = new Priority
                    {
                        Id = task.TaskDetail.TaskPriority.Id.ToString(),
                        Name = task.TaskDetail.TaskPriority.Name
                    },
                    AssignedUser = new UserAssignment
                    {
                        Id = task.TaskDetail.AssignedUser.Id.ToString(),
                        UserName = task.TaskDetail.AssignedUser.Username
                    },
                    Location = location
                }
            };
        }
    }
}
