using MediatR;
using SnagIt.API.Core.Application.Authorisation;
using SnagIt.API.Core.Application.Exceptions;

namespace SnagIt.API.Core.Application.Behaviours
{
    public class AuthorisationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IAuthoriseRequestPolicy<TRequest>> _authorisationPolicies;

        public AuthorisationBehavior(IEnumerable<IAuthoriseRequestPolicy<TRequest>> authorisationPolicies)
        {
            _authorisationPolicies = authorisationPolicies ?? throw new ArgumentNullException(nameof(authorisationPolicies));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            foreach (var policy in _authorisationPolicies)
            {
                var result = await policy.Authorise(request, cancellationToken);
                if (!result.IsAuthorised)
                {
                    throw new AuthorisationException(result.FailureMessage);
                }
            }

            return await next();
        }
    }
}
