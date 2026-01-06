using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

public static class LogExtensions
{
    public static string Tag(
        this object source,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string typeName = source.GetType().Name;

        try
        {
            var frameMethod = new StackFrame(1, false).GetMethod();
            if (frameMethod != null)
            {
                typeName = GetLogicalDeclaringTypeName(frameMethod) ?? typeName;
            }
        }
        catch
        {
            // never let logging break logging
        }

        return $"[{typeName}__{memberName}] Line: {lineNumber}";
    }

    private static string? GetLogicalDeclaringTypeName(MethodBase method)
    {
        // Normal case (non-async, non-generated)
        var declaringType = method.DeclaringType;

        // If we’re in an async state machine, declaringType will look like "<Foo>d__10"
        // and the real parent is stored in AsyncStateMachineAttribute.
        var asyncAttr = method.GetCustomAttribute<AsyncStateMachineAttribute>();
        if (asyncAttr?.StateMachineType != null)
        {
            // The state machine is a nested type inside the real declaring type.
            return asyncAttr.StateMachineType.DeclaringType?.Name
                   ?? asyncAttr.StateMachineType.Name;
        }

        // If we’re *inside* MoveNext() of the generated state machine,
        // method won’t have AsyncStateMachineAttribute. In that case:
        if (declaringType != null && declaringType.IsNested
            && declaringType.Name.StartsWith("<", StringComparison.Ordinal))
        {
            // Often nested inside the real declaring type
            return declaringType.DeclaringType?.Name;
        }

        return declaringType?.Name;
    }
}
