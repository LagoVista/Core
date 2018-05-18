using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequestListener
    {
        void Start();
    }
}
