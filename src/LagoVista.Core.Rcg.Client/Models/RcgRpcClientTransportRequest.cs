using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgRpcClientTransportRequest
    {
        public string TargetInstanceId { get; set; }
        public RemoteControlFrame Frame { get; set; }
        public int TimeoutSeconds { get; set; }

        public RcgRpcClientTransportRequest()
        {
            TargetInstanceId = String.Empty;
            Frame = new RemoteControlFrame();
            TimeoutSeconds = 30;
        }
    }
}
