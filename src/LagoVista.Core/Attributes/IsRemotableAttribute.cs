using System;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Usage: Indicates that the decorated interface contains async messaging methods for run-time proxy generation. 
    /// All methods will be registered for use by the remote proxy and request broker unless marked by AsyncIgnoreAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class AsyncMessagingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AsyncIgnoreAttribute : Attribute
    {
    }
}
