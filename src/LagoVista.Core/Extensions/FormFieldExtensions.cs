// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8a475e8abfb3b9e28e67db02a5014da89f0ce003b31cc158a940ba37f93736d6
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    public static class FormFieldExtensions
    {
        public static bool Validate(this FormField value)
        {
            return true;
        }

        public static string ToFieldKey(this string memberName)
        {
            return $"{memberName.Substring(0, 1).ToLower()}{memberName.Substring(1)}";
        }
    }
}
