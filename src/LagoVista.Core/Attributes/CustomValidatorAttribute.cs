// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 978747b206cfd93e2809fb714a6ba8d0ea0659a5fc19649654aeb79bee5e2b9f
// IndexVersion: 2
// --- END CODE INDEX META ---
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
