using SnagIt.API.Core.Application.Models.User;
using SnagIt.API.Core.Domain.Aggregates.User;
using static SnagIt.API.Core.Application.Models.User.UserDto;


namespace SnagIt.API.Core.Application.Extensions.Mapping.User
{
    public static class SnagItUserExtensions
    {
        public static UserDetailItem ToUserDetailItem(this SnagItUser user)
        {
            var assignedProperties = user.AssignedProperties.Select(x =>
            {
                return new UserDto.PropertyAssignment
                {
                    Property = new PropertyId { 
                        Id = x.Property.Id, 
                        Name = x.Property.Name 
                    },
                    Role = new UserRole
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name
                    }
                };
            }).ToList();

            return new UserDetailItem
            {
                Id = user.Id.ToString(),
                Kind = nameof(SnagItUser),
                Item = new UserDetail
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserDetail.UserName,
                    FullName = user.UserDetail.FullName,
                    FirstName = user.UserDetail.FirstName,
                    LastName = user.UserDetail.LastName,
                    Email = user.UserDetail.Email,
                    PropertyAssignments = assignedProperties
                }
            };
        }
    }
}
