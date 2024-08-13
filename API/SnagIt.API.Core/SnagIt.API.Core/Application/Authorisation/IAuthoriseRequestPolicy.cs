

namespace SnagIt.API.Core.Application.Authorisation
{
    public interface IAuthoriseRequestPolicy<T>
    {
        Task<AuthorisationResult> Authorise(T request, CancellationToken cancellation);
    }
}
