using System;
using System.Net;
using LagoVista.Core.Networking.Interfaces;
using Newtonsoft.Json;

namespace LagoVista.Core.Networking.Models
{
    public enum ResponeStatus
    {
        ClientException = -1,
        Ok = 1,
        Failed = 0
    }


    public class APIResponse 
    {
        public HttpStatusCode StatusCode { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public ResponeStatus Status { get; protected set; }
        public Exception Exception { get; protected set; }        

        public bool Success { get { return Status == ResponeStatus.Ok; } }

        public static APIResponse CreateOK()
        {
            return new APIResponse()
            {
                 StatusCode = HttpStatusCode.OK,
                 Status = ResponeStatus.Ok,
            };
        }

        public static APIResponse CreateForException(Exception ex)
        {
            return new APIResponse()
            {
                Exception = ex,
                StatusCode = HttpStatusCode.ExpectationFailed,
                Status = ResponeStatus.Failed,
            };
        }

        public static APIResponse CreateFailed(HttpStatusCode statusCode)
        {
            return new APIResponse()
            {
                StatusCode = statusCode,
                Status = ResponeStatus.Failed,
            };
        }
    }


    public class APIResponse<TResult> : APIResponse where TResult: class
    {
        public APIResponse(TResult result)
        {
            Result = result;
            Status = ResponeStatus.Ok;
        }

        public TResult Result { get; private set; }

        public static APIResponse<TResult> Create(TResult result)
        {
            return new APIResponse<TResult>(result)
            {
                Status = ResponeStatus.Ok,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static APIResponse<TResult> Create(String json)
        {
            var result = JsonConvert.DeserializeObject<TResult>(json);
            return new APIResponse<TResult>(result)
            {
                Status = ResponeStatus.Ok,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static APIResponse<TResult> FromFailedStatusCode(HttpStatusCode statusCode)
        {
            return new APIResponse<TResult>(null)
            {
                StatusCode = statusCode,
                Status = ResponeStatus.Failed,
            };
        }

        public static APIResponse<TResult> FromException(Exception ex)
        {
            return new APIResponse<TResult>(null)
            {
                Exception = ex,
                StatusCode = HttpStatusCode.ExpectationFailed,
                Status = ResponeStatus.Failed,
            };
        }
    }
}
