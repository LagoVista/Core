// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 45d451267b33ef63dd8cfe35cc391e1b4a959f94d65afc6d0f9fece92cba5f3a
// IndexVersion: 0
// --- END CODE INDEX META ---
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
