using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Exceptions
{
    public class InvalidDataException : System.Exception
    {
        public InvalidDataException([CallerMemberName] string area = null,  string[] errors = null)  : base("SystemInvalidDataException")
        {
            Area = area;
            if (errors != null)
            {
                ValidationErrors = errors.ToList();
            }
        }        

        public List<String> ValidationErrors { get; private set; }

        public string Area { get; }
        
    }

    public class ValidationFailedException : System.Exception
    {

        public ValidationFailedException(ValidationResult result)
        {
            Result = result;
        }

        public ValidationResult Result { get; private set; }
        
    }
}
