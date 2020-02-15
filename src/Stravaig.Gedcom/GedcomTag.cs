using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public struct GedcomTag
    {
        private const char Underscore = (char) 0x95;
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
            return _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GedcomTag tag)
                return _value.Equals(tag._value);

            return false;
        }

        public static bool operator ==(GedcomTag a, GedcomTag b) => a._value == b._value;
        public static bool operator !=(GedcomTag a, GedcomTag b) => a._value != b._value;
    }
}