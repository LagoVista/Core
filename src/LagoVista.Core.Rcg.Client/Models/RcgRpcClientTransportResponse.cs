using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgRpcClientTransportResponse
    {
        public string TargetInstanceId { get; set; }
        public string CorrelationId { get; set; }
        public RemoteControlFrame Frame { get; set; }

        public RcgRpcClientTransportResponse()
        {
            TargetInstanceId = String.Empty;
            CorrelationId = String.Empty;
            Frame = new RemoteControlFrame();
        }
    }
}
