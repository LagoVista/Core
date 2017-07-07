using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Models
{
 //   [EntityDescription(Name: "Authentication Response", Description: "The Authentication Response will be returned from the server in give a authentication reqeust.", Domain: Domains.AuthenticationDomain)]

    public class AuthResponse
    {
        public AuthResponse()
        {
            Roles = new List<string>();
        }

        public string AccessToken { get; set; }

        public string AccessTokenExpiresUTC { get; set;}

        public string RefreshToken { get; set; }

        public string RefreshTokenExpiresUTC { get; set; }

        public string AppInstanceId { get; set; }

        public List<String> Roles { get; set; }
    }
}
