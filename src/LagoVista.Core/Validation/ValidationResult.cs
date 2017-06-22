using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Validation
{
    public class ValidationResult
    {
        public String ResultId { get; set; }


        public ValidationResult()
        {
            Warnings = new List<ErrorMessage>();
            Errors = new List<ErrorMessage>();
            ResultId = Guid.NewGuid().ToId();
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

        public void Concat(ValidationResult result)
        {
            Errors.AddRange(result.Errors);
            Warnings.AddRange(result.Warnings);
        }

        public InvokeResult ToActionResult()
        {
            return new InvokeResult()
            {
                Errors = Errors,
                Warnings = Warnings,
            };
        }

        public static InvokeResult FromException(String tag, Exception ex)
        {
            var result = new InvokeResult();
            result.Errors.Add(new ErrorMessage()
            {
                ErrorCode = "EXC9999",
                Message = "Unhandled Excpetion",
                Details = tag
            });

            result.Errors.Add(new ErrorMessage()
            {
                ErrorCode = "EXC9998",
                Message = ex.Message,
                Details = ex.StackTrace
            });

            if (ex.InnerException != null)
            {
                result.Errors.Add(new ErrorMessage()
                {
                    ErrorCode = "EXC9997",
                    Message = ex.InnerException.Message,
                    Details = ex.InnerException.StackTrace
                });
            }

            return result;
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
