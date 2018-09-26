using System;
using System.Collections.Generic;

namespace LagoVista.Core.Retry.Sample
{
    internal static class ErrorCodesCanRetryMap
    {
        static ErrorCodesCanRetryMap()
        {
            var errorCodeType = typeof(ErrorCodes);
            var retryAttributeType = typeof(SampleRetryAllowedAttribute);

            foreach(var memberInfo in errorCodeType.GetMembers())
            {
                _canRetryMap[(ErrorCodes)Enum.Parse(errorCodeType, memberInfo.Name)] = 
                    memberInfo.GetCustomAttributes(retryAttributeType, false).Length != 0;
            }
        }

        private static readonly Dictionary<ErrorCodes, bool> _canRetryMap = new Dictionary<ErrorCodes, bool>();

        internal static bool CanRetry(ErrorCodes errorCode)
        {
            return _canRetryMap[errorCode];
        }
    }

    internal class SampleRetryTester : IRetryExceptionTester
    {
        public bool CanRetry(Exception exception)
        {
            return exception is SampleException && ErrorCodesCanRetryMap.CanRetry(((SampleException)exception).ErrorCode);
        }
    }
}
