using LagoVista.Core.Interfaces;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IAuthManager
    {
        string AuthToken { get; set; }
        long AuthTokenExpiration { get; set; }
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        bool IsAuthenticated { get; set; }
        bool IsUserVerified { get; set; }
        string RefreshToken { get; set; }
        long RefreshTokenExpiration { get; set; }
        IAppUser User { get; set; }

        Task LoadAsync();
        Task PersistAsync();
    }
}
