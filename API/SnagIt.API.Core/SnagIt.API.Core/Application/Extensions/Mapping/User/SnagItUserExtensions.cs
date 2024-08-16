using Azure.Core;
using SnagIt.API.Core.Application.Models.Shared;
using SnagIt.API.Core.Application.Models.User;
using SnagIt.API.Core.Domain.Aggregates.User;
using SnagIt.API.Core.Infrastructure.Repositiories.Blob.Clients;
using static SnagIt.API.Core.Application.Models.User.UserDto;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SnagIt.API.Core.Application.Extensions.Mapping.User
{
    public static class SnagItUserExtensions
    {
        private static IIsolatedBlobClient? _isolatedBlobClient;

        public static void InitBlobClient(IIsolatedBlobClient isolatedBlobClient)
        {
            _isolatedBlobClient = isolatedBlobClient ?? throw new ArgumentNullException(nameof(isolatedBlobClient));
        }

        public static async Task<UserDetailItem> ToUserDetailItem(this SnagItUser user)
        {
            if (_isolatedBlobClient is null)
            {
                throw new ArgumentNullException(nameof(_isolatedBlobClient));
            }

            var assignedProperties = user.AssignedProperties.Select(async x =>
            {
                var imageUri = x.ImageUri is null ?
                    null :
                    await _isolatedBlobClient.GetReadToken(x.ImageUri);

                return new UserDto.PropertyAssignment
                {
                    Property = new PropertyId { 
                        Id = x.Property.Id, 
                        Name = x.Property.Name 
                    },
                    ImageUri = imageUri,
                    Role = new UserRole
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name
                    },
                    Location = new LocationGetDto
                    {
                        Latitude = x.LocationDetail.Latitude,
                        Longitude = x.LocationDetail.Longitude,
                        Elevation = x.LocationDetail.Elevation
                    }
                };
            }).ToList();

            var properties = await Task.WhenAll(assignedProperties);

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
                    PropertyAssignments = properties.ToList()
                }
            };
        }
    }
}
