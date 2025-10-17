// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 48f11e06128d60f1e0bec01443bdabd8382005a5dc2750371f291bbd919b33cb
// IndexVersion: 1
// --- END CODE INDEX META ---
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

        public string ErrorMessage
        {
            get
            {
                return Errors.Count == 0 ? "No Error" : Errors.First().Message;
            }
        }

        public bool Successful { get { return Errors.Count == 0; } }
        public List<ErrorMessage> Warnings { get; protected set; }
        public List<ErrorMessage> Errors { get; protected set; }

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


        public virtual InvokeResult ToInvokeResult()
        {
            return new InvokeResult()
            {
                Errors = Errors,
                Warnings = Warnings,
            };
        }
    }
}
