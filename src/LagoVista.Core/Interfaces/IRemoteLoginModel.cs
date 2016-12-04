using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IRemoteLoginModel
    {
        string Email { get; set; }
        string GrantType { get; set; }
        string Password { get; set; }
        string RefreshToken { get; set; }
    }
}
