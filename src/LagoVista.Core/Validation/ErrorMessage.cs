// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3522461a742c58d267882b09bc9d6ca1262bab5d5cc67a00ef3c49294a4fa967
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        public string Context { get; set; }                

        public string Details { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
