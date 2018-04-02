using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{

    /// <summary>
    /// Prevalidation allows you to update the object prior to calling the entire validation routine this is useful for setting default properties or cleaning up unused values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PreValidationAttribute : Attribute
    {
    }
}
