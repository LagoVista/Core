using System;

namespace LagoVista.Core.Validation
{
    public class ErrorMessage
    {
        public ErrorMessage()
        {

        }

        public ErrorMessage(String message, bool systemError = false)
        {
            Message = message;
            SystemError = systemError;
        }

        public ErrorMessage(String errorCode, String message, bool systemError = false)
        {
            ErrorCode = errorCode;
            Message = message;
            SystemError = systemError;
        }

        public string ErrorCode { get; set; }

        public bool SystemError { get; set; }


        public string Message { get; set; }

        public string Details { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
