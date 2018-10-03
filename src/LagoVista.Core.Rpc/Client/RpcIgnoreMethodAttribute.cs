using System;

namespace LagoVista.Core.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcIgnoreMethodAttribute : Attribute
    {
    }
}
