using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Models
{
    [EntityDescription(Name:"Authentication Request", Description:"The Authentication Request contains fields necessary to authenticate against a server resource or get a refresh token.", Domain:Domains.AuthenticationDomain)]
    public class AuthRequest : IRemoteLoginModel
    {
        public string GrantType { get; set; }
        public String Email { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String RefreshToken { get; set; }
    }
}
