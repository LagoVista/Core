using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Retry.Sample
{
    internal class Sample : ISample
    {
        /// <summary>
        /// echo echos
        /// </summary>
        /// <param name="value"></param>
        /// <returns>value</returns>
        public string Echo(string value)
        {
            return value;
        }

        /// <summary>
        /// EchoAsync echos and shows how async code is supported
        /// </summary>
        /// <param name="value"></param>
        /// <returns>value</returns>
        public async Task<string> EchoAsync(string value)
        {
            return await Task.Run(() => { return Echo(value); });
        }

        /// <summary>
        /// ping pings
        /// </summary>
        /// <returns>utc now</returns>
        public DateTime Ping()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// throws a non-recoverable exception
        /// </summary>
        public void ThrowNonRetriableException()
        {
            throw new SampleException(ErrorCodes.RuntimeError, "Memory buffer overwrite. Abort recomende%6*# _#2@/'}:-(|`~");
        }

        /// <summary>
        /// throws an an exception that is a good candiate for retry
        /// </summary>
        public void ThrowRetriableException()
        {
            throw new SampleException(ErrorCodes.NetworkError, "Transient network error encountered. Try again.");
        }
    }
}
