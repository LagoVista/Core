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
