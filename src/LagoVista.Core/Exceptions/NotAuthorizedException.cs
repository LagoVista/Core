using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(AuthorizeResult response) : base()
        {
            AuthorizationResponse = response;
        }

        public NotAuthorizedException(String reason) : base(reason)
        {
            Reason = reason;
        }

        public AuthorizeResult AuthorizationResponse { get; private set; }

        public String Reason { get; private set; }
    }

    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException(String reason): base(reason)
        {
            Reason = reason;
        }

        public String Reason { get; private set; }
    }
    
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(String reason) : base(reason)
        {
            Reason = reason;
        }

        public String Reason { get; private set; }
    }
}
