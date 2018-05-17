using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Networking.Rpc
{
    /* This is used so we can somewhere create a class that has connnections settings specific to service bus (or whatever */
    public interface IProxyConnectionSettings
    {
        IConnectionSettings ConnectionSettings { get; }
    }
}
