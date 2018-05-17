using System;

namespace LagoVista.Core.Attributes
{
    /// <summary>
    /// Usage: Interface contains remotable methods for run-time proxy generation. All methods will be used in remote proxy unless marked by ExcludeFromRemoteProxyAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class IsRemotableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ExcludeFromProxyAttribute : Attribute
    {
    }
}
