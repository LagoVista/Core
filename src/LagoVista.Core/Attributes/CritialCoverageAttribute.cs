using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Method |
        AttributeTargets.Constructor |
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class CriticalCoverageAttribute : Attribute
    {
    }
}
