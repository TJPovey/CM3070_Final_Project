using MediatR;
using SnagIt.API.Core.Domain.Aggregates.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Domain.Events.UserAggregate
{
    public class UserPropertyAssignedDomainEvent : INotification
    {
        private UserPropertyAssignedDomainEvent(SnagItUser snagItUser, PropertyAssignment propertyAssignment)
        {
            User = snagItUser;
            PropertyAssignment = propertyAssignment;
        }

        public static UserPropertyAssignedDomainEvent Create(SnagItUser snagItUser, PropertyAssignment propertyAssignment)
            => new UserPropertyAssignedDomainEvent(snagItUser, propertyAssignment);

        public SnagItUser User { get; }

        public PropertyAssignment PropertyAssignment { get; }
    }
}
