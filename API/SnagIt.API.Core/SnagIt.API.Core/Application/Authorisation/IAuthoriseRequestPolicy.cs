using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Application.Authorisation
{
    public interface IAuthoriseRequestPolicy<T>
    {
        Task<AuthorisationResult> Authorise(T request, CancellationToken cancellation);
    }
}
