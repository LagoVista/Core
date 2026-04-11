using System;

namespace LagoVista.Core.Attributes
{
    [Flags]
    public enum AiSchemaContext
    {
        None = 0,
        ToolInput = 1,
        ToolOutput = 2,
        ArtifactAuthoring = 4,
        ArtifactExtraction = 8,
        UiAssist = 16,
        InternalAdmin = 32,
        All = ToolInput | ToolOutput | ArtifactAuthoring | ArtifactExtraction | UiAssist | InternalAdmin
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AiSchemaIgnoreAttribute : Attribute
    {
        public AiSchemaIgnoreAttribute(AiSchemaContext contexts = AiSchemaContext.All)
        {
            Contexts = contexts;
        }

        public AiSchemaContext Contexts { get; }
    }
}

