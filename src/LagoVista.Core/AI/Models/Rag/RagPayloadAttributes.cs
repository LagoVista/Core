using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class QdrantPayloadIndexAttribute : Attribute
    {
        public QdrantPayloadIndexAttribute(QdrantPayloadIndexKind kind)
        {
            Kind = kind;
        }

        public QdrantPayloadIndexKind Kind { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RagRequiredAttribute : Attribute
    {
        public RagRequiredAttribute(string message = null)
        {
            Message = message;
        }

        public string Message { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RagNotDefaultAttribute : Attribute
    {
        public RagNotDefaultAttribute(string message = null)
        {
            Message = message;
        }

        public string Message { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RagDefaultValueAttribute : Attribute
    {
        public RagDefaultValueAttribute(object value, string warning = null)
        {
            Value = value;
            Warning = warning;
        }

        public object Value { get; }

        public string Warning { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RagMinimumAttribute : Attribute
    {
        public RagMinimumAttribute(long minimum)
        {
            Minimum = minimum;
        }

        public long Minimum { get; }
    }
}
