using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Attributes
{

    /// <summary>
    /// Usage: Method must accept ValidationResult and populate it with custom validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomValidatorAttribute : Attribute
    {

    }
}
