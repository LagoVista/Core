using System;
using System.Text;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgRuntimeCommandContext
    {
        public RcgRuntimeChannelConnection Connection { get; set; }
        public RemoteControlFrame Frame { get; set; }

        public RcgRuntimeCommandContext()
        {
            Connection = new RcgRuntimeChannelConnection();
            Frame = new RemoteControlFrame();
        }

        public string CorrelationId
        {
            get { return Frame.CorrelationId; }
        }

        public string Method
        {
            get { return Frame.Method; }
        }

        public string ContentType
        {
            get { return Frame.ContentType; }
        }

        public string PayloadBase64
        {
            get { return Frame.PayloadBase64; }
        }

        public byte[] GetPayloadBytes()
        {
            if (String.IsNullOrWhiteSpace(Frame.PayloadBase64))
            {
                return new byte[0];
            }

            return Convert.FromBase64String(Frame.PayloadBase64);
        }

        public string GetPayloadText()
        {
            return Encoding.UTF8.GetString(GetPayloadBytes());
        }
    }
}
