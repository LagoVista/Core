// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 184d8d695ea8c63de6eb59225969087abf205f274e0728d7ff3b0e895f87e8e1
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        public bool OrgMismatch { get; set; } = false;

        public EntityHeader CurrentOrg { get; set; }
        public EntityHeader EntityOrg { get; set; }
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
