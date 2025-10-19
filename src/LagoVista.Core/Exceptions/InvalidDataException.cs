// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7b8e3b6150c7a192777d45d7575b62a5c3f4a37d10064d39d6981eb5cdd97dbf
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        public InvalidDataException([CallerMemberName] string area = null, params  string[] errors)  : base("SystemInvalidDataException")
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
