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

        public void AddUserError(String error, string context = null)
        {
            Errors.Add(new ErrorMessage(error) { Context = context });
        }
        public void AddSystemError(String error, string context = null)
        {
            Errors.Add(new ErrorMessage(error, true) { Context = context });
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


        public InvokeResult ToInvokeResult()
        {
            return new InvokeResult()
            {
                Errors = Errors,
                Warnings = Warnings,
            };
        }
    }
}
