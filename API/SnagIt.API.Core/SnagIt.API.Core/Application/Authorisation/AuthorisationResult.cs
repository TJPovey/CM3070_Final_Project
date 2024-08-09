

namespace SnagIt.API.Core.Application.Authorisation
{
    public class AuthorisationResult
    {
        private AuthorisationResult(bool isAuthorised, string failureMessage)
        {
            IsAuthorised = isAuthorised;
            FailureMessage = failureMessage;
        }

        public static AuthorisationResult Failure() => new AuthorisationResult(false, string.Empty);

        public static AuthorisationResult Failure(string failureMessage) => new AuthorisationResult(false, failureMessage);

        public static AuthorisationResult Success() => new AuthorisationResult(true, string.Empty);

        public bool IsAuthorised { get; }

        public string FailureMessage { get; }
    }
}
