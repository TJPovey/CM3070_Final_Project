using SnagIt.API.Core.Application.Features.Shared.Models;


namespace SnagIt.API.Core.Application.Models.User
{
    public class UserDto : Response
    {
        public override ResponseData Data { get; set; }

        public class UserDetailItem : ResponseItem<UserDetail>
        {
            public override UserDetail Item { get; set; }
        }

        public class UserDetail
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public List<PropertyAssignment> PropertyAssignments { get; set; }
        }

        public class PropertyAssignment
        {
            public PropertyId Property { get; set; }

            public UserRole Role { get; set; }
        }

        public class PropertyId
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class UserRole
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
