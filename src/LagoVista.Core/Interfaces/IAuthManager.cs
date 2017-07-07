using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IAuthManager
    {
        string AccessToken { get; set; }
        string AccessTokenExpirationUTC { get; set; }
        string AppInstanceId { get; set; }
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        bool IsAuthenticated { get; set; }
        string RefreshToken { get; set; }
        string RefreshTokenExpirationUTC { get; set; }
        UserInfo User { get; set; }
        Task LoadAsync();
        Task PersistAsync();
        Task LogoutAsync();

        List<String> Roles { get; set; }
    }
}
