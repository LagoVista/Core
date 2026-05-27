using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IConfigServerClientSettings
    {
        string PublicKeySetUrl { get; }
        string AppKey { get;  }
        string Environment { get; }
        string AuthorizationToken { get; }
    }
}
