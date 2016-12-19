using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Models
{
 //   [EntityDescription(Name: "Authentication Response", Description: "The Authentication Response will be returned from the server in give a authentication reqeust.", Domain: Domains.AuthenticationDomain)]

    public class AuthResponse : ILoginResponse
    {
        public string AuthToken { get; set; }

        public long AuthTokenExpiration { get; set;}

        public string RefreshToken { get; set; }

        public long RefreshTokenExpiration { get; set; }

        public string TokenType { get; set; }
    }
}
