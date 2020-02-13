using System;
using System.Globalization;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public struct GedcomLevel
    {
        private const char Zero = (char) 0x30; // number 0.

        private readonly int _value;

        public GedcomLevel(int value)
        {
            if (value < 0 || value > 99)
                throw new ArgumentOutOfRangeException(nameof(value), value, "A GEDCOM level can only be 0 to 99 inclusive.");

            _value = value;
        }

        public GedcomLevel(GedcomLevel level)
        {
            _value = level._value;
        }

        public GedcomLevel(string value)
        {
            if (!IsValid(value))
                throw new ArgumentException("The passed value is not a valid GEDCOM level.", nameof(value));

            _value = int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static bool IsValid(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                return false;

            if (target.Length > 2)
                return false;
            
            if (target.Length == 1 && target[0].IsGedcomDigit())
                return true;

            return (target[0].IsGedcomDigit() && target[1].IsGedcomDigit() && target[0] != Zero);
        }

        public bool CanFollowFrom(GedcomLevel previousLineLevel)
        {
            return this._value <= previousLineLevel._value + 1;
        }

        public bool CanBeFollowedBy(GedcomLevel nextLineLevel)
        {
            return nextLineLevel.CanFollowFrom(this);
        }
        
        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is GedcomLevel level))
                return false;

            return _value == level._value;
        }
        
        public static bool operator ==(GedcomLevel a, GedcomLevel b) => a._value == b._value;
        public static bool operator !=(GedcomLevel a, GedcomLevel b) => a._value != b._value;
    }
}