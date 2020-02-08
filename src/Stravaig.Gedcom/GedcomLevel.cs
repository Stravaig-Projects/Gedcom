using System;
using System.Globalization;

namespace Stravaig.Gedcom
{
    public struct GedcomLevel
    {
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