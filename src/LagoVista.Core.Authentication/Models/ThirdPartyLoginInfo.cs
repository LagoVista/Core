// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b8fef68728e0dd88607aec24ca78fddc27c7db1ecc428baf7fe331c35a993f0c
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;

namespace LagoVista.Core.Authentication.Models
{
 //   [EntityDescription(Name: "Third Party Login Info", Description: "In some cases the user may not login with the primary authentication mechanism.  The Thrid Party Login Information will contain the d detail to associate a LagoVista account with a Third Party Account.", Domain: Domains.AuthenticationDomain)]
    public class ThirdPartyLoginInfo
    {
        public string LoginId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string DisplayName { get; set; }
    }
}
