using MediatR;
using SnagIt.API.Core.Domain.Aggregates.User;


namespace SnagIt.API.Core.Domain.Events.UserAggregate
{
    public class UserCreatedDomainEvent : INotification
    {
        private UserCreatedDomainEvent(SnagItUser snagItUser)
        {
            SnagItUser = snagItUser;
        }

        public static UserCreatedDomainEvent Create(SnagItUser snagItUser)
            => new UserCreatedDomainEvent(snagItUser);

        public SnagItUser SnagItUser { get; }
    }
}
