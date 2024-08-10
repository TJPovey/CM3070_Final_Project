using SnagIt.API.Core.Application.Features.Shared.Models;


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
            public List<UserAssignment> UserAssignments { get; set; }
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
