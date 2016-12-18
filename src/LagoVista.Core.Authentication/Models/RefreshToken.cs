using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Models
{
    [EntityDescription(Name: "Refresh Token", Domain: Domains.AuthenticationDomain)]
    public class RefreshToken
    {
        public string Subject { get; set; }
        public string ClientId { get; set; }
        public String IssuedUtc { get; set; }
        public String ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
        public bool Enabled { get; set; }

        public RefreshToken()
        {
            Enabled = true;
        }
    }
}
