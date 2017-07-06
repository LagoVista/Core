using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Authentication.Models
{
//    [EntityDescription(Name: "Refresh Token", Domain: Domains.AuthenticationDomain)]
    public class RefreshToken : TableStorageEntity
    {
        public RefreshToken(string userId)
        {
            PartitionKey = userId;
        }

        public string AppId { get; set; }
        public string ClientId { get; set; }
        public String IssuedUtc { get; set; }
        public String ExpiresUtc { get; set; }
     }
}
