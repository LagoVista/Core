using LagoVista.Core.PlatformSupport;
using System;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;

namespace LagoVista.Core.Authentication.Managers
{
    public class AuthManager : IAuthManager
    {
        private const string AUTH_MGR_SETTINGS = "AUTHSETTINGS.JSON";
        IStorageService _storage;
        IDeviceInfo _deviceInfo;

        public AuthManager(IStorageService storage, IDeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
            _storage = storage;
        }

        public string AuthToken { get; set; }
        public long AuthTokenExpiration { get; set; }
        public string DeviceId { get; set; }
        public String DeviceType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string RefreshToken { get; set; }
        public long RefreshTokenExpiration { get; set; }
        public UserInfo User { get; set; }

        public async Task LoadAsync()
        {
            //TODO: Might move to automapper if we have more stuff like this.
            var storedAuthmanager = await _storage.GetAsync<AuthManager>(AUTH_MGR_SETTINGS);
            if (storedAuthmanager == null)
            {
                DeviceId = _deviceInfo.DeviceUniqueId;
                DeviceType = _deviceInfo.DeviceType;
                IsAuthenticated = false;
            }
            else
            {
                AuthToken = storedAuthmanager.AuthToken;
                AuthTokenExpiration = storedAuthmanager.AuthTokenExpiration;
                DeviceId = storedAuthmanager.DeviceId;
                DeviceType = storedAuthmanager.DeviceType;
                IsAuthenticated = storedAuthmanager.IsAuthenticated;
                RefreshToken = storedAuthmanager.RefreshToken;
                RefreshTokenExpiration = storedAuthmanager.RefreshTokenExpiration;
                User = storedAuthmanager.User;
            }
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task PersistAsync()
        {
            return _storage.StoreAsync<AuthManager>(this, AUTH_MGR_SETTINGS);
        }
    }
}
