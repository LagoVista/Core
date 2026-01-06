using System.IO;
using System.Runtime.CompilerServices;

public static class LogExtensions
{
    public static string Tag(
        this object source,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        return $"[{source.GetType().Name}__{memberName}] Line: {lineNumber}";
    }
}
