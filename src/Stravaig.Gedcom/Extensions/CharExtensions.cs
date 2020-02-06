namespace Stravaig.Gedcom.Extensions
{
    public static class CharExtensions
    {
        private const char Zero = (char) 0x30;
        private const char Nine = (char) 0x39;
        private const char LowerAlpha = (char) 0x61; 
        private const char LowerZulu = (char) 0x7A; 
        private const char UpperAlpha = (char) 0x41; 
        private const char UpperZulu = (char) 0x5A; 
        private const char Underscore = (char) 0x5F; 
        
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
        
        /// <summary> 
        /// Determines if the target value is a GEDCOM alpha.
        /// </summary> 
        /// <param name="target">The target character to test.</param> 
        /// <returns>true if the target is a GEDCOM alpha.</returns> 
        /// <remarks> 
        /// alpha:= 
        ///   [(0x41)-(0x5A) | (0x61)-(0x7A) | (0x5F) ] 
        ///   where: 
        ///   (0x41)-(0x5A)=A to Z 
        ///   (0x61)-(0x7A)=a to z 
        ///   (0x5F)=(_) underscore 
        /// </remarks> 
        public static bool IsGedcomAlpha(this char target) 
        { 
            return (target >= LowerAlpha && target <= LowerZulu) || 
                   (target >= UpperAlpha && target <= UpperZulu) || 
                   target == Underscore; 
        } 
    }
}