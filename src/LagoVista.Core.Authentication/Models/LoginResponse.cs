using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Authentication.Models
{
    public class LoginResponse : ILoginResponse
    {
        public string AuthToken { get; set; }

        public long AuthTokenExpiration { get; set;}

        public string RefreshToken { get; set; }

        public long RefreshTokenExpiration { get; set; }

        public string TokenType { get; set; }
    }
}
