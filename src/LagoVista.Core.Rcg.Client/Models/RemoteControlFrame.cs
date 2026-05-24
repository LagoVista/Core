using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RemoteControlFrame
    {
        public string FrameType { get; set; }
        public string CorrelationId { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public string PayloadBase64 { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }

        public RemoteControlFrame()
        {
            FrameType = String.Empty;
            CorrelationId = String.Empty;
            Method = String.Empty;
            ContentType = "application/json";
            PayloadBase64 = String.Empty;
            ErrorCode = String.Empty;
            ErrorMessage = String.Empty;
            CreatedUtc = DateTimeOffset.UtcNow;
        }

        public static RemoteControlFrame CreatePong(RemoteControlFrame ping)
        {
            if (ping == null) throw new ArgumentNullException(nameof(ping));

            return new RemoteControlFrame
            {
                FrameType = RemoteControlFrameTypes.Pong,
                CorrelationId = ping.CorrelationId,
                Method = ping.Method,
                ContentType = ping.ContentType,
                PayloadBase64 = ping.PayloadBase64
            };
        }

        public static RemoteControlFrame CreateError(RemoteControlFrame request, string errorCode, string errorMessage)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new RemoteControlFrame
            {
                FrameType = RemoteControlFrameTypes.Error,
                CorrelationId = request.CorrelationId,
                Method = request.Method,
                ErrorCode = errorCode ?? String.Empty,
                ErrorMessage = errorMessage ?? String.Empty
            };
        }
    }
}
