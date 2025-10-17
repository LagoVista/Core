// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0e64edba8464d351c0cb5ebb01ab4fad420e40fb518fbf5c7cdccdd5bf29b4ff
// IndexVersion: 1
// --- END CODE INDEX META ---
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
        public string AppInstanceId { get; set; }
        public String IssuedUtc { get; set; }
        public String ExpiresUtc { get; set; }
     }
}
