using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Rcg.Client.Models;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Services
{
    public partial class RcgRuntimeChannelClient
    {
        public async Task<InvokeResult> ListenAsync(RcgRuntimeChannelConnection connection, IRcgRuntimeCommandHandler handler, CancellationToken cancellationToken)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (connection.Socket == null) throw new ArgumentNullException(nameof(connection.Socket));

            while (!cancellationToken.IsCancellationRequested && connection.Socket.State == WebSocketState.Open)
            {
                var frame = await ReceiveFrameAsync(connection, cancellationToken);
                if (frame == null)
                {
                    break;
                }

                if (String.Equals(frame.FrameType, RemoteControlFrameTypes.Ping, StringComparison.OrdinalIgnoreCase))
                {
                    await SendFrameAsync(connection, RemoteControlFrame.CreatePong(frame), cancellationToken);
                    continue;
                }

                if (String.Equals(frame.FrameType, RemoteControlFrameTypes.Close, StringComparison.OrdinalIgnoreCase))
                {
                    await connection.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Remote control channel closed by gateway.", cancellationToken);
                    break;
                }

                if (String.Equals(frame.FrameType, RemoteControlFrameTypes.Command, StringComparison.OrdinalIgnoreCase))
                {
                    var response = await handler.HandleAsync(new RcgRuntimeCommandContext
                    {
                        Connection = connection,
                        Frame = frame
                    }, cancellationToken);

                    if (response != null)
                    {
                        await SendFrameAsync(connection, response, cancellationToken);
                    }
                }
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> SendFrameAsync(RcgRuntimeChannelConnection connection, RemoteControlFrame frame, CancellationToken cancellationToken)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            if (connection.Socket == null) throw new ArgumentNullException(nameof(connection.Socket));

            if (connection.Socket.State != WebSocketState.Open)
            {
                return InvokeResult.FromError("Remote control runtime channel is not connected.");
            }

            var json = JsonConvert.SerializeObject(frame);
            var bytes = Encoding.UTF8.GetBytes(json);
            await connection.Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
            return InvokeResult.Success;
        }

        private async Task<RemoteControlFrame> ReceiveFrameAsync(RcgRuntimeChannelConnection connection, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                var buffer = new byte[8192];
                WebSocketReceiveResult result;

                do
                {
                    result = await connection.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return null;
                    }

                    stream.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                var json = Encoding.UTF8.GetString(stream.ToArray());
                if (String.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<RemoteControlFrame>(json);
            }
        }
    }
}
