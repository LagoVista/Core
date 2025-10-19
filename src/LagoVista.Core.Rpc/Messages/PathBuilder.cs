// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6a617b4f44e955d8b16b6d80a6ef11af270b4a98847c9cc637a09ff0b9fe0a11
// IndexVersion: 0
// --- END CODE INDEX META ---
using System.Reflection;

namespace LagoVista.Core.Rpc.Messages
{
    public static class PathBuilder
    {
        public static string BuildPath(MethodInfo methodInfo)
        {
            return $"{methodInfo.DeclaringType.FullName}/{methodInfo.Name}/";
        }
    }
}
