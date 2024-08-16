using SnagIt.API.Core.Application.Models.Shared;
using SnagIt.API.Core.Domain.Aggregates.Property;
using static SnagIt.API.Core.Application.Models.Property.PropertyDto;


namespace SnagIt.API.Core.Application.Extensions.Mapping.Property;

public static class SnagItPropertyExtensions
{
    public static PropertyDetailItem ToPropertyDetailItem(
        this SnagItProperty snagItProperty, Uri? writeToken, Uri? imageUri)
    {
        var assignedProperties = snagItProperty.AssignedUsers.Select(x =>
        {
            return new UserAssignment
            {
                Id = x.UserId.Id.ToString(),
                UserName = x.UserId.Username,
                Role = new UserRole
                {
                    Id = x.Role.Id.ToString(),
                    Name = x.Role.Name
                }
            };
        }).ToList();

        var assignedTasks = snagItProperty.AssignedTasks.Select(x =>
        {
            var taskLocation = x.TaskId.LocationDetail is null ?
                null :
                new LocationGetDto
                {
                    Latitude = x.TaskId.LocationDetail.Latitude,
                    Longitude = x.TaskId.LocationDetail.Longitude
                };

            return new TaskAssignment
            {
                Id = x.TaskId.Id.ToString(),
                Open = x.TaskId.Open,
                Name = x.TaskId.Name,
                TaskCategory = new TaskCategory
                {
                    Id = x.TaskId.TaskCategory.Id.ToString(),
                    Name = x.TaskId.TaskCategory.Name
                },
                TaskPriority = new TaskPriority
                {
                    Id = x.TaskId.TaskPriority.Id.ToString(),
                    Name = x.TaskId.TaskPriority.Name
                },
                Location = taskLocation
            };
        }).ToList();

        return new PropertyDetailItem
        {
            Kind = nameof(PropertyDetailItem),
            Id = snagItProperty.Id.ToString(),
            Item = new PropertyDetail
            {
                Id = snagItProperty.Id.ToString(),
                PropertyName = snagItProperty.PropertyDetail.PropertyName,
                WriteToken = writeToken,
                ImageUri = imageUri,
                ReportTitle = snagItProperty.PropertyDetail.ReportTitle,
                Location = new LocationGetDto
                {
                    Latitude = snagItProperty.PropertyDetail.LocationDetail.Latitude,
                    Longitude = snagItProperty.PropertyDetail.LocationDetail.Longitude,
                    Elevation = snagItProperty.PropertyDetail.LocationDetail.Elevation,
                },
                UserAssignments = assignedProperties,
                TaskAssignments = assignedTasks,
                OwnerId = new UserRole
                {
                    Id = snagItProperty.OwnerId.Id.ToString(),
                    Name = snagItProperty.OwnerId.Username
                }
            },
        };
    }
}
