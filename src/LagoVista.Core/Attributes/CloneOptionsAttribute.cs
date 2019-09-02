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
