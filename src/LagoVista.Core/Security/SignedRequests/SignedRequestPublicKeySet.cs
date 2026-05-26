using System;
using System.Collections.Generic;

namespace LagoVista.Core.Security
{
    public class SignedRequestPublicKeySet
    {
        public string Environment { get; set; }
        public string Version { get; set; }
        public DateTimeOffset GeneratedUtc { get; set; }
        public IList<SignedRequestPublicKeyEntry> Keys { get; set; }

        public SignedRequestPublicKeySet()
        {
            Environment = String.Empty;
            Version = String.Empty;
            GeneratedUtc = DateTimeOffset.MinValue;
            Keys = new List<SignedRequestPublicKeyEntry>();
        }
    }
}
