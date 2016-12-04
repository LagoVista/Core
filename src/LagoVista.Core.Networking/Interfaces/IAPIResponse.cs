using System;
using System.Net;

namespace LagoVista.Core.Networking.Interfaces
{
    public enum ResponeStatus
    {
        ClientException = -1,
        Ok = 1,        
        Failed = 0
    }


    public interface IAPIResponse
    {
        bool Success { get; }
        ResponeStatus Status { get; }
        HttpStatusCode StatusCode { get; }
        String ErrorMessage { get; }
        Exception Exception { get; }
    }

    public interface IAPIResponse<TResult> : IAPIResponse where TResult : class
    {
        TResult Result { get; }
    }

}
