using Newtonsoft.Json;

namespace LagoVista.Core.Networking.Models
{
    public sealed class Json
    {
        public Json(string value) { Value = value; }

        public string Value { get; } = null;

        public bool HasValue { get { return !string.IsNullOrEmpty(Value); } }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// using the string to json implicit operator just recreates the problem of using string in the messaging classes for the json constructors
        /// </remarks>
        /// <param name="json"></param>
        //public static implicit operator Json(string value) => new Json(value);

        public static implicit operator string(Json json) => json.Value;
    }
}
