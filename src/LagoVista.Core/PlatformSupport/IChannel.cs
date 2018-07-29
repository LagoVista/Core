using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IChannel
    {
        Task OpenAsync();
        bool IsConnected { get; }
        Task CloseAsync();

        Task WriteAsync(string msg);
        Task WriteAsync(byte[] buffer);
        Task<int> ReadAsync(byte[] bufffer, int start, int size, CancellationToken token = default(CancellationToken));

    }
}
