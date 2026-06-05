using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class EntityAuthoringContextAttribute : Attribute
    {
        public EntityAuthoringContextAttribute(string context)
        {
            Context = context;
        }

        public string Context { get; }
    }
}
