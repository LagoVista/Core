﻿using System;
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
