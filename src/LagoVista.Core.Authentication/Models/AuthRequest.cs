using LagoVista.Core.Interfaces;
using System;

namespace LagoVista.Core.Authentication.Models
{
    //  [EntityDescription(Name:"Authentication Request", Description:"The Authentication Request contains fields necessary to authenticate against a server resource or get a refresh token.", Domain:Domains.AuthenticationDomain)]
    public class AuthRequest
    {
        public AuthTypes AuthType { get; set; } = AuthTypes.User;

        /// <summary>
        /// Grant type supported are password and refreshtoken
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// Regsitered App Id for Calling Web Service
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Unique ID from the physical hardware (if available) 
        /// </summary>
        public String DeviceId {get; set; }

        public String DeviceRepoId { get; set; }

        /// <summary>
        /// Unqiue Generated ID generated for the installation, will be generated from
        /// server on initial install if not passed in as a parameter, if passed in
        /// as a parameter we will assume the user is just logging in for a second time.
        /// </summary>
        public string AppInstanceId { get; set; }

        /// <summary>
        /// Type of client, free form text but should be something like iPhone/Android/Windows/Web, etc...
        /// </summary>
        public string ClientType { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }

        /// <summary>
        /// If Present will login with the context of this user id, if not will use the login from the last logino
        /// </summary>
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public string InstanceId { get; set; }
        public string InstanceAuthKey { get; set; }
    }
}
                                                                                                                                                                                                                                                                                                                                                                