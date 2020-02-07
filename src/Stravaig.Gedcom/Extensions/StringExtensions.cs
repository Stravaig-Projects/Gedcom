namespace Stravaig.Gedcom.Extensions
{
    public static class StringExtensions
    {
        private const char PointerBookend = (char) 0x40; // @ symbol.
        
        /// <summary>
        /// Determines if the target string is a valid pointer identifier.
        /// </summary>
        /// <param name="target">The string to check.</param>
        /// <returns>true, if this matches the format of a GEDCOM pointer/cross-reference ID.</returns>
        /// <remarks>
        /// The cross-reference ID has a maximum of 22 characters, including the enclosing ‘at’ signs (@).
        /// </remarks>
        public static bool IsGedcomPointer(this string target)
        {
            // pointer:=
            //   [(0x40) + alphanum + pointer_string + (0x40) ]
            //   where:
            //   (0x40)=@

            if (target == null)
                return false;

            var len = target.Length;
            if (len < 3 || len > 22)
                return false;

            if (target[0] != PointerBookend || target[len - 1] != PointerBookend)
                return false;

            if (!target[1].IsGedcomAlphanum())
                return false;
            
            for (int i = 2; i < len - 2; i++)
            {
                if (!target[i].IsGedcomNonAt())
                    return false;
            }

            return true;
        }
    }
}