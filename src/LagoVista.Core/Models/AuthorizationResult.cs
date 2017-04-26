using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{



    public class AuthorizeResult
    {
        public enum AuthorizeActions
        {
            Create,
            Read,
            Update,
            Delete,
            GetForOrgs,
            Perform
        }


        public bool IsAuthorized { get; private set; }
        public static AuthorizeResult Authorized { get { return new AuthorizeResult() { IsAuthorized = true }; } }

        public InvokeResult ToActionResult()
        {
            return new InvokeResult()
            {

            };
        }
    }

}
