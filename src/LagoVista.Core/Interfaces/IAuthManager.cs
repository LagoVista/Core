using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IAuthManager
    {
        string AuthToken { get; set; }
        long AuthTokenExpiration { get; set; }
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        bool IsAuthenticated { get; set; }
        string RefreshToken { get; set; }
        long RefreshTokenExpiration { get; set; }
        UserInfo User { get; set; }
        Task LoadAsync();
        Task PersistAsync();
        Task LogoutAsync();
    }
}
