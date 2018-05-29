using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequestValidator
    {
        void ValidateArguments(IAsyncRequest request, ParameterInfo[] parameters);
    }
}
