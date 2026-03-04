using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class AllowLegacyGuidDocumentIdAttribute : Attribute { }
}
