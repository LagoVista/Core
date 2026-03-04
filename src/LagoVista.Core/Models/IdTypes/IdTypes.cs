
using System;

namespace LagoVista.Core
{
    /// <summary>
    /// Bridge ID that normalizes to GuidString36 (relational).
    /// Accepts: GuidString36, NormalizedId32, or string.
    /// </summary>
    public readonly struct RelationalId : IEquatable<RelationalId>
    {
        private readonly GuidString36 _value;
        public GuidString36 Value => _value;

        public RelationalId(GuidString36 value)
        {
            _value = value;
        }

        public RelationalId(NormalizedId32 value)
        {
            _value = value.ToGuidString36(); // blows up loudly if not guid-derived
        }

        public RelationalId(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (GuidString36.IsStrictLowerD(value))
            {
                _value = new GuidString36(value);
                return;
            }

            if (NormalizedId32.IsNormalizedId32(value))
            {
                _value = new NormalizedId32(value).ToGuidString36();
                return;
            }

            throw new FormatException(
                $"Invalid RelationalId: '{value}'. Expected either GuidString36 (lowercase dashed GUID) " +
                "or NormalizedId32 (32 chars A-Z0-9).");
        }

        /// <summary>
        /// Compares a <see cref="RelationalId"/> with a <see cref="Guid"/>.
        /// </summary>
        public static bool operator ==(RelationalId left, Guid right) => left.Value.ToGuid() == right;

        /// <summary>
        /// Determines inequality between a <see cref="RelationalId"/> and a <see cref="Guid"/>.
        /// </summary>
        public static bool operator !=(RelationalId left, Guid right) => left.Value.ToGuid() != right;

        /// <summary>
        /// Compares a <see cref="Guid"/> with a <see cref="RelationalId"/>.
        /// </summary>
        public static bool operator ==(Guid left, RelationalId right) => left == right.Value.ToGuid();

        /// <summary>
        /// Determines inequality between a <see cref="Guid"/> and a <see cref="RelationalId"/>.
        /// </summary>
        public static bool operator !=(Guid left, RelationalId right) => left != right.Value.ToGuid();

        public Guid ToGuid() => _value.ToGuid();

        public Guid DbId { get => _value.ToGuid(); }

        public override string ToString() => _value.ToString();

        public bool Equals(RelationalId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is RelationalId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();

        public static implicit operator GuidString36(RelationalId id) => id._value;

        // Convenience at the bridge boundary only (safe: this type is unambiguous)
        public static implicit operator RelationalId(string value) => new RelationalId(value);
        public static implicit operator RelationalId(GuidString36 value) => new RelationalId(value);
        public static implicit operator RelationalId(NormalizedId32 value) => new RelationalId(value);
    }

    /// <summary>
    /// Bridge ID that normalizes to NormalizedId32 (document).
    /// Accepts: NormalizedId32, GuidString36, or string.
    /// </summary>
    public readonly struct DocumentId : IEquatable<DocumentId>
    {
        private readonly NormalizedId32 _value;
        public NormalizedId32 Value => _value;

        public DocumentId(NormalizedId32 value)
        {
            _value = value;
        }

        public DocumentId(GuidString36 value)
        {
            _value = value.ToNormalizedId32();
        }

        public DocumentId(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (NormalizedId32.IsNormalizedId32(value))
            {
                _value = new NormalizedId32(value);
                return;
            }

            if (GuidString36.IsStrictLowerD(value))
            {
                _value = new GuidString36(value).ToNormalizedId32();
                return;
            }

            throw new FormatException(
                $"Invalid DocumentId: '{value}'. Expected either NormalizedId32 (32 chars A-Z0-9) " +
                "or GuidString36 (lowercase dashed GUID).");
        }

        public override string ToString() => _value.ToString();

        public bool Equals(DocumentId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DocumentId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();

        public static implicit operator NormalizedId32(DocumentId id) => id._value;

        public static implicit operator DocumentId(string value) => new DocumentId(value);
        public static implicit operator DocumentId(NormalizedId32 value) => new DocumentId(value);
        public static implicit operator DocumentId(GuidString36 value) => new DocumentId(value);
    }
}
