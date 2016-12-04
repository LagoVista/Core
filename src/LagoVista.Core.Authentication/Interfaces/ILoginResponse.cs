using Newtonsoft.Json;
using System;

namespace LagoVista.Core.Interfaces
{
    public interface ILoginResponse
    {
        [JsonProperty("access_token")]
        String AuthToken { get; set; }
        [JsonProperty("token_type")]
        String TokenType { get; set; }
        [JsonProperty("refresh_token")]
        String RefreshToken { get; set; }
        [JsonProperty("expires_inX")]
        long AuthTokenExpiration { get; set; }
        long RefreshTokenExpiration { get; set; }
    }
}
