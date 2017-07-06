using System;

namespace LagoVista.Core.Authentication.Models
{
    //  [EntityDescription(Name:"Authentication Request", Description:"The Authentication Request contains fields necessary to authenticate against a server resource or get a refresh token.", Domain:Domains.AuthenticationDomain)]
    public class AuthRequest
    {
        /// <summary>
        /// Grant type supported are password and refreshtoken
        /// </summary>
        public string GrantType { get; set; }
        
        /// <summary>
        /// Regsitered App Id for Calling Web Service
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Unique Client/Device ID.  To ensure sessions are maintained this should be unique to device
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Type of client, free form text but should be something like iPhone/Android/Windows/Web, etc...
        /// </summary>
        public string ClientType { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}
                                                                                                                                                                                                                                                                                                                                                                