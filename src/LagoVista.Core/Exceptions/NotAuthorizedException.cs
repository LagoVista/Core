using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Authentication.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(AuthorizeResult response)
        {
            AuthorizationResponse = response;
        }

        public AuthorizeResult AuthorizationResponse { get; private set; }
    }
}
