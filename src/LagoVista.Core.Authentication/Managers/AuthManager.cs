using LagoVista.Core.PlatformSupport;
using System;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System.Collections.Generic;

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
            Roles = new List<string>();
        }

        public string AccessToken { get; set; }
        public string AccessTokenExpirationUTC { get; set; }
        public string DeviceId { get; set; }
        public String DeviceType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string RefreshToken { get; set; }
        public string RefreshTokenExpirationUTC { get; set; }
        public UserInfo User { get; set; }
        public List<String> Roles { get; set; }

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
                AccessToken = storedAuthmanager.AccessToken;
                AccessTokenExpirationUTC = storedAuthmanager.AccessTokenExpirationUTC;
                DeviceId = storedAuthmanager.DeviceId;
                DeviceType = storedAuthmanager.DeviceType;
                IsAuthenticated = storedAuthmanager.IsAuthenticated;
                RefreshToken = storedAuthmanager.RefreshToken;
                RefreshTokenExpirationUTC = storedAuthmanager.RefreshTokenExpirationUTC;
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
