using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Application.Exceptions
{
    [Serializable]
    public class AuthorisationException : Exception
    {
        public AuthorisationException() { }

        public AuthorisationException(string message) : base(message) { }

        public AuthorisationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
