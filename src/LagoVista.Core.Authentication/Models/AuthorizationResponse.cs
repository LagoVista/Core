using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Authentication.Models
{
    public class AuthorizationResponse
    {

        public bool IsAuthorized { get; private set; }
        public static AuthorizationResponse Authorized { get { return new AuthorizationResponse() { IsAuthorized = true }; } }

        public InvokeResult ToActionResult()
        {
            return new InvokeResult()
            {

            };
        }
    }
}
