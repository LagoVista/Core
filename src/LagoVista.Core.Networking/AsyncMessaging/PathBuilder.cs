using System.Reflection;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public static class PathBuilder
    {
        public static string BuildPath(MethodInfo methodInfo)
        {
            return $"{methodInfo.DeclaringType.FullName}/{methodInfo.Name}/";
        }
    }
}
