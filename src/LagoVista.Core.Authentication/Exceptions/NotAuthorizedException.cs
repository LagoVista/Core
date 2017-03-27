using LagoVista.Core.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Authentication.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(AuthorizationResponse response)
        {
            AuthorizationResponse = response;
        }

        public AuthorizationResponse AuthorizationResponse { get; private set; }
    }
}
