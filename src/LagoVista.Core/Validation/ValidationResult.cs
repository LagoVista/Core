using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            Warnings = new List<ErrorMessage>();
            Errors = new List<ErrorMessage>();
        }

        public bool Successful { get { return Errors.Count == 0; } }
        public List<ErrorMessage> Warnings { get; private set; }
        public List<ErrorMessage> Errors { get; private set; }

        internal void AddUserError(String error)
        {
            Errors.Add(new ErrorMessage(error));
        }
        internal void AddSystemError(String error)
        {
            Errors.Add(new ErrorMessage(error, true));
        }

        public InvokeResult<T> ToActionResult<T>(T result)
        {
            return new InvokeResult<T>()
            {
                Errors = Errors,
                Result = result,
                Warnings = Warnings,
            };
        }

        public InvokeResult ToActionResult()
        {
            return new InvokeResult()
            {
                Errors = Errors,
                Warnings = Warnings,
            };
        }
    }

    public class InvokeResult<T> : ValidationResult
    {
        public T Result { get; set; }

        
    }

    public class InvokeResult : ValidationResult
    {
        public static InvokeResult Success
        {
            get { return new InvokeResult(); }
        }
    }

    public class ErrorMessage
    {
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

        public string ErrorCode { get; private set; }

        public bool SystemError { get; private set; }


        public string Message { get; private set; }

        public string Details { get;  set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
