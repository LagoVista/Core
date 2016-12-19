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
