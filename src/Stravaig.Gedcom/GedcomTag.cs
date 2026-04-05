using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public readonly struct GedcomTag : IComparable<GedcomTag>, IEquatable<GedcomTag>
    {
        private const char Underscore = (char) 0x5F;
        private readonly string _value;

        public GedcomTag(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            if (!value.IsGedcomTag())
                throw new ArgumentException("Value is not formatted correctly for a GEDCOM Tag.", nameof(value));

            _value = value;
        }

        public bool IsUserDefined => _value[0] == Underscore;
        
        public GedcomTag(GedcomTag tag)
        {
            _value = tag._value;
        }

        public override string ToString()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            return obj is GedcomTag other && Equals(other);
        }

        public static bool operator ==(GedcomTag a, GedcomTag b) => a._value == b._value;
        public static bool operator !=(GedcomTag a, GedcomTag b) => a._value != b._value;

        public int CompareTo(GedcomTag other)
        {
            return string.Compare(_value, other._value, StringComparison.Ordinal);
        }

        public bool Equals(GedcomTag other)
        {
            return _value.Equals(other._value, StringComparison.Ordinal);
        }
    }
}
