using System;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.Extensions
{
    public static class StringExtensions
    {
        private const char PointerBookend = (char) 0x40; // @ symbol.
        private const char Zero = (char) 0x30; // number 0.
        
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

        /// <summary>
        /// Determines if the string is a valid GEDCOM tag.
        /// </summary>
        /// <param name="target">The string to check.</param>
        /// <returns>true, if this matches the format of a GEDCOM tag.</returns>
        /// <remarks>
        /// The length of the GEDCOM TAG is a maximum of 31 characters,
        /// with the first 15 characters being unique.
        /// tag:= [alphanum | tag + alphanum ]
        /// </remarks>
        public static bool IsGedcomTag(this string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                return false;
            if (target.Length > 31)
                return false;
            
            for(int i=0; i<target.Length; i++)
                if (!target[i].IsGedcomAlphanum())
                    return false;

            return true;
        }

        public static bool IsGedcomLevel(this string target)
        {
            return GedcomLevel.IsValid(target);
        }

        public static GedcomTag AsGedcomTag(this string target)
        {
            return new GedcomTag(target);
        }

        public static GedcomPointer AsGedcomPointer(this string target)
        {
            return new GedcomPointer(target);
        }

        public static GedcomLine AsGedcomLine(this string target)
        {
            GedcomLineParser parser = new GedcomLineParser();
            return parser.Parse(target);
        }

        public static GedcomSex AsGedcomSex(this string target)
        {
            switch (target)
            {
                case "M":
                    return GedcomSex.Male;
                case "F":
                    return GedcomSex.Female;
                case "U":
                    return GedcomSex.Undetermined;
                default:
                    return GedcomSex.NotKnown;
            }
        }
    }
}