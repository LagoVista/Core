// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7857a62d2a23dd05c09d6ea4a95d64d6be43b4b10150baae22d574bc9fa8c4f5
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Exceptions
{
    public class ValidationException : System.Exception
    {
        public ValidationException(String message, List<ErrorMessage> errs) : base(message)
        {
            Errors = errs;
        }

        public ValidationException(String message, ErrorMessage err) : base(message)
        {
            Errors = new List<ErrorMessage>() { err };
        }

        public ValidationException(String message, bool systemError, string err) : base(message)
        {
            Errors = new List<ErrorMessage>() { new ErrorMessage(err, systemError)  };
        }
  
        public List<ErrorMessage> Errors { get; private set; }
    }
}
