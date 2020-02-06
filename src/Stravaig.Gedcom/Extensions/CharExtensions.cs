namespace Stravaig.Gedcom.Extensions
{
    public static class CharExtensions
    {
        private const char Zero = (char) 0x30;
        private const char Nine = (char) 0x39;

        /// <summary>
        /// Determines if the target value is a GEDCOM Digit.
        /// </summary>
        /// <param name="target">The target character to test.</param>
        /// <returns>true if the target is a GEDCOM digit.</returns>
        /// <remarks>
        /// digit:=
        ///   [(0x30)-(0x39) ]
        ///   where:
        ///   (0x30)-(0x39) = One of the digits 0, 1,2,3,4,5,6,7,8,9
        /// </remarks>
        public static bool IsGedcomDigit(this char target)
        {
            return (target >= Zero) && (target <= Nine);
        }
    }
}