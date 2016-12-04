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
        public ValidationException(String message, List<ValidationMessage> errs) : base(message)
        {
            Errors = errs;
        }

        public ValidationException(String message, ValidationMessage err) : base(message)
        {
            Errors = new List<ValidationMessage>() { err };
        }

        public ValidationException(String message, bool systemError, string err) : base(message)
        {
            Errors = new List<ValidationMessage>() { new ValidationMessage(err, systemError)  };
        }
  
        public List<ValidationMessage> Errors { get; private set; }
    }
}
