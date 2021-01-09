using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public readonly struct GedcomPointer : IComparable<GedcomPointer>
    {
        private readonly string _value;

        public GedcomPointer(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            if (!value.IsGedcomPointer())
                throw new ArgumentException("Value is not formatted correctly for a GEDCOM pointer.", nameof(value));

            _value = value;
        }

        public GedcomPointer(GedcomPointer pointer)
        {
            _value = pointer._value;
        }

        public override string ToString()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GedcomPointer pointer)
                return _value.Equals(pointer._value);

            return false;
        }

        public static bool operator ==(GedcomPointer a, GedcomPointer b) => a._value == b._value;
        public static bool operator !=(GedcomPointer a, GedcomPointer b) => a._value != b._value;

        public int CompareTo(GedcomPointer other)
        {
            return string.Compare(_value, other._value, StringComparison.Ordinal);
        }
    }
}