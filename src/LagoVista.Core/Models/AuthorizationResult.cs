// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 547b7f8925215d54808324822efc15897c3ba55eb84e521f76ad54e9a93668e9
// IndexVersion: 2
// --- END CODE INDEX META ---
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
