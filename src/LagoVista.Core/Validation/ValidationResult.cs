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
            Warnings = new List<ValidationMessage>();
            Errors = new List<ValidationMessage>();
        }

        public bool IsValid { get { return Errors.Count == 0; } }
        public List<ValidationMessage> Warnings { get; private set; }
        public List<ValidationMessage> Errors { get; private set; }

        internal void AddUserError(String error)
        {
            Errors.Add(new ValidationMessage(error));
        }
        internal void AddSystemError(String error)
        {
            Errors.Add(new ValidationMessage(error, true));
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

    }

    public class ValidationMessage
    {
        public ValidationMessage(String message, bool systemError = false)
        {
            Message = message;
            SystemError = systemError;
        }

        public bool SystemError { get; private set; }


        public string Message { get; private set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
