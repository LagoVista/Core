using System;
using System.Net.WebSockets;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgRuntimeChannelConnection : IDisposable
    {
        public RemoteControlWelcome Welcome { get; set; }
        public ClientWebSocket Socket { get; set; }

        public RcgRuntimeChannelConnection()
        {
            Welcome = new RemoteControlWelcome();
        }

        public bool IsConnected
        {
            get { return Socket != null && Socket.State == WebSocketState.Open; }
        }

        public void Dispose()
        {
            if (Socket != null)
            {
                Socket.Dispose();
                Socket = null;
            }
        }
    }
}
