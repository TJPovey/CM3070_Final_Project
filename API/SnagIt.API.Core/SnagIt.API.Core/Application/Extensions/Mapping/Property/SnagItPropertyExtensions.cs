using SnagIt.API.Core.Domain.Aggregates.Property;
using static SnagIt.API.Core.Application.Models.Property.PropertyDto;


namespace SnagIt.API.Core.Application.Extensions.Mapping.Property;

public static class SnagItPropertyExtensions
{
    public static PropertyDetailItem ToPropertyDetailItem(this SnagItProperty snagItProperty)
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

        return new PropertyDetailItem
        {
            Kind = nameof(PropertyDetailItem),
            Id = snagItProperty.Id.ToString(),
            Item = new PropertyDetail
            {
                Id = snagItProperty.Id.ToString(),
                PropertyName = snagItProperty.PropertyDetail.PropertyName,
                ReportTitle = snagItProperty.PropertyDetail.ReportTitle,
                UserAssignments = assignedProperties
            },
        };
    }
}
