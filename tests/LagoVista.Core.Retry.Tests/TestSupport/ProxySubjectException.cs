// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b2113a7992763d297bd8f87b541ea5fa0b513dc8409404935869532555c7a012
// IndexVersion: 2
// --- END CODE INDEX META ---
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
