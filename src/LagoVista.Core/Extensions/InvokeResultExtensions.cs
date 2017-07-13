using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    public static class InvokeResultExtensions
    {
        public static KeyValuePair<string, string> ToKVP(this string value, string key)
        {
            return new KeyValuePair<string, string>(key, value);
        }

        public static InvokeResult ToInvokeResult(this string error)
        {
            return InvokeResult.FromErrors(new ErrorMessage(error));
        }
    }
}
