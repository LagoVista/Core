// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: caf64fbed70f2b8a783a0e9701a1c9f20fabdae750a0c2ef565efee4a61c09b9
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CloneOptionsAttribute : Attribute
    {
        public CloneOptionsAttribute(bool autoClone)
        {
            AutoClone = autoClone;
        }

        public bool AutoClone { get; }
    }
}
