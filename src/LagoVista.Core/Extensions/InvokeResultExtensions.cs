// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 45d451267b33ef63dd8cfe35cc391e1b4a959f94d65afc6d0f9fece92cba5f3a
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
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

        public static KeyValuePair<string, string> ToKVP(this NormalizedId32 value, string key)
        {
            return new KeyValuePair<string, string>(key, value.Value);
        }

        public static KeyValuePair<string, string> ToKVP(this int value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this CalendarDate value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this UtcTimestamp value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this ClockTime value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }


        public static KeyValuePair<string, string> ToKVP(this LagoVistaKey value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this Guid value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this Double value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this Decimal value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static KeyValuePair<string, string> ToKVP(this EntityHeader value, string key)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        public static InvokeResult ToInvokeResult(this string error)
        {
            return InvokeResult.FromErrors(new ErrorMessage(error));
        }
    }
}
