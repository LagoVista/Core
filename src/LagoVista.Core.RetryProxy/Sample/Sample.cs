// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 887fcbc53f5b97052cfca0bcbecfda0c11d8f0cae8f8a909645116128b563cf5
// IndexVersion: 2
// --- END CODE INDEX META ---
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
