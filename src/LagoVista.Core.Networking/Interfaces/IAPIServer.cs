using System;
using LagoVista.Core.Networking.Models;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IApiHandler
    {

    }

    public class MethodHandlerAttribute : Attribute
    {
        public MethodHandlerAttribute(MethodTypes methodType)
        {
            MethodType = methodType;
            FullPath = null;
        }

        public enum MethodTypes
        {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            TRACE,
            CONNECT,
            SUBSCRIBE
        }

        public MethodTypes MethodType { get; private set; }

        public String FullPath { get; set; }

    }

    public interface IAPIServer
    {
        event EventHandler<HttpRequestMessage> MessageReceived;

        int Port { get; }        

        void StartServer(int port);

        bool ShowDiagnostics { get; set; }

        void RegisterAPIHandler(IApiHandler handler);
    }
}
