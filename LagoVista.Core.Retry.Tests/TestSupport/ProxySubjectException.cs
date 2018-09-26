using System;

namespace LagoVista.Core.Retry
{
    internal class ProxySubjectTestException : Exception
    {
        public ProxySubjectTestException()
        {
        }

        public ProxySubjectTestException(string message) : base(message)
        {
        }

        public ProxySubjectTestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
