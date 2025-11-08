// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 296b9636422d49d8fe694077ed630057b2896fe051ab29a2b9c07a175f658580
// IndexVersion: 2
// --- END CODE INDEX META ---
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
