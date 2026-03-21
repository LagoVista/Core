using LagoVista.Core.Validation;
using System;

namespace LagoVista
{ 
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(Error errorCode) : base(errorCode.Message)
        {
            Error = errorCode;
        }

        public InvalidConfigurationException(string message) : base(message)
        {
            Error = new Error() { Message = message };
        }

        public InvalidConfigurationException(Error errorCode, string details) : base(errorCode.Message)
        {
            Error = errorCode;
            Error.Details = details;
        }

        public InvalidConfigurationException(ErrorCode errCode) : this(errCode, null)
        {

        }

        public InvalidConfigurationException(ErrorCode errorCode, string details) : base(errorCode.Message)
        {
            Error = errorCode.ToError();
            Error.Details = details;
        }

        public InvalidConfigurationException(ErrorCode errorCode, Type configurationType, string id, string details = "") : base(errorCode.Message)
        {
            Error = errorCode.ToError();
            Error.ConfigurationId = id;
            Error.ConfigurationType = configurationType.FullName;
            Error.Details = details;
        }

        public Error Error { get; private set; }

        public static InvalidConfigurationException FromErrorCode(ErrorCode errorCode)
        {
            return new InvalidConfigurationException(errorCode);
        }

        public static InvalidConfigurationException FromErrorCode(ErrorCode errorCode, string details)
        {
            return new InvalidConfigurationException(errorCode, details);
        }

        public InvokeResult ToFailedInvocation()
        {
            var result = new InvokeResult();
            var errMessage = new ErrorMessage(Error.ErrorCode, Error.Message);
            result.Errors.Add(errMessage);
            return result;
        }
    }
}
