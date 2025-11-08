// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3f8c5d4aadaa4d824a9366722fadb0013414885097218469e68eac6d3940bbe8
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Retry.Sample
{
    internal interface ISample
    {
        /// <summary>
        /// echo echos
        /// </summary>
        /// <param name="value"></param>
        /// <returns>value</returns>
        string Echo(string value);

        /// <summary>
        /// EchoAsync echos and shows how async code is supported
        /// </summary>
        /// <param name="value"></param>
        /// <returns>value</returns>
        Task<string> EchoAsync(string value);

        /// <summary>
        /// ping pings
        /// </summary>
        /// <returns>date time</returns>
        DateTime Ping();

        /// <summary>
        /// throws a non-recoverable exception
        /// </summary>
        void ThrowRetriableException();

        /// <summary>
        /// throws an an exception that is a good candiate for retry
        /// </summary>
        void ThrowNonRetriableException();
    }
}
