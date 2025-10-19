// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f11b3a57d69521c3bb2957e24c0359ef5eb4fd98477696a650aa0aa9232ecdc3
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        private readonly IStorageService _storage;
        private readonly IDeviceInfo _deviceInfo;

        public event EventHandler<EntityHeader> OrgChanged;
        public event EventHandler<List<EntityHeader>> RolesChanged;

        public AuthManager(IStorageService storage, IDeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
            _storage = storage;
            Roles = new List<EntityHeader>();
        }

        public string AccessToken { get; set; }
        public string AccessTokenExpirationUTC { get; set; }
        public String AppInstanceId { get; set; }
        public string DeviceId { get; set; }
        public String DeviceType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string RefreshToken { get; set; }
        public string RefreshTokenExpirationUTC { get; set; }
        public UserInfo User { get; set; }
        public List<EntityHeader> Roles { get; set; }

        public void RaiseOrgChanged(EntityHeader newOrg)
        {
            OrgChanged?.Invoke(this, newOrg);
        }

        public void RaiseRolesChanged(List<EntityHeader> newRoles)
        {
            RolesChanged?.Invoke(this, newRoles);
        }

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
