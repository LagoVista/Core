using System.Reflection;

namespace LagoVista.Core.Networking.Rpc.Messages
{
    internal static class PathBuilder
    {
        public static string BuildPath(MethodInfo methodInfo)
        {
            return $"{methodInfo.DeclaringType.FullName}/{methodInfo.Name}/";
        }
    }
}
