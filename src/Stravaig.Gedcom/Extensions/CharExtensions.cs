using System.Linq;

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
        private const char Space = (char) 0x20; 
        private const char NumberSign = (char) 0x23; 

        private static readonly char[] AnselAsUnicodeChars = new char[]
        {
            // A0 - AF range
            (char)0x0141, // Ł 
            (char)0x00D8, // Ø
            (char)0x0110, // Đ
            (char)0x00DE, // Þ
            (char)0x00C6, // Æ
            (char)0x0152, // Œ
            (char)0x02B9, // ʹ
            (char)0x00B7, // ·
            (char)0x226D, // ♭
            (char)0x00AE, // ®
            (char)0x00B1, // ± 
            (char)0x01A0, // Ơ
            (char)0x01AF, // Ư
            (char)0x02BC, // ʼ
            
            // B0 - BF range
            (char)0x02BB, // ʻ
            (char)0x0142, // ł
            (char)0x00F8, // ø
            (char)0x0111, // đ
            (char)0x00FE, // þ
            (char)0x00E6, // æ
            (char)0x0153, // œ
            (char)0x02BA, // ʺ
            (char)0x0131, // ı
            (char)0x00A3, // £
            (char)0x00F0, // ð
            (char)0x01A1, // ơ
            (char)0x01B0, // ư
            		
            // C0 - CF range
            (char)0x00B0, // °
            (char)0x2113, // ℓ
            (char)0x2117, // ℗
            (char)0x00A9, // ©
            (char)0x266F, // ♯
            (char)0x00BF, // ¿
            (char)0x00A1, // ¡
            
            // E0 - EF range
            (char)0x0309, // ̉ 
            (char)0x0300, //  ̀ 
            (char)0x0301, //  ́ 
            (char)0x0302, //  ̂ 
            (char)0x0303, //  ̃ 
            (char)0x0304, //  ̄ 
            (char)0x0306, //  ̆ 
            (char)0x0307, //  ̇ 
            (char)0x0308, //  ̈ 
            (char)0x030C, //  ̌ 
            (char)0x030A, //  ̊ 
            (char)0xFE20, // ︠ 
            (char)0xFE21, // ︡ 
            (char)0x0315, // ̕ 
            (char)0x030B, //  ̋ 
            (char)0x0310, // ̐ 
            
            // F0 - FF range
            (char)0x0327, //  ̧ 
            (char)0x0328, // ̨ 
            (char)0x0323, // ̣ 
            (char)0x0324, // ̤ 
            (char)0x0325, // ̥ 
            (char)0x0333, // ̳ 
            (char)0x0332, // ̲ 
            (char)0x0326, // ̦ 
            (char)0x031C, // ̜ 
            (char)0x032E, // ̮ 
            (char)0xFE22, // ︢ 
            (char)0xFE23, // ︣ 
            (char)0x0313, //  ̓ 
            
            // GEDCOM extensions
            (char)0x25A1, // □ empty box
            (char)0x25A0, // ■ black box
            (char)0x0065, // e midline e
            (char)0x006F, // o midline o
            (char)0x00Df, // ß es zet
            (char)0x0338, // ̸  diacritic slash through char
        };
        
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

        /// <summary>
        /// Determines if the target value is a GEDCOM alphanum.
        /// </summary>
        /// <param name="target">The target character to test.</param>
        /// <returns>true if the target is a GEDCOM alphanum</returns>
        /// <remarks>
        /// alphanum:=
        ///   [alpha | digit ]
        /// </remarks>
        public static bool IsGedcomAlphanum(this char target)
        {
            return target.IsGedcomDigit() || target.IsGedcomAlpha();
        }

        /// <summary>
        /// Determines if the target value is a GEDCOM otherchar.
        /// </summary>
        /// <param name="target">The target character to test.</param>
        /// <returns>true if the target is a GEDCOM otherchar</returns>
        /// <remarks>
        /// otherchar:=
        ///   [(0x21)-(0x22) | (0x24)-(0x2F) | (0x3A)-(0x3F) | (0x5B)-(0x5E) | (0x60) | (0x7B)-(0x7E) |
        ///   (0x80)-(0xFE)]
        ///   where, respectively:
        ///   (0x21)-(0x22)=! "
        ///   (0x24)-(0x2F)=$ % & ' ( ) * + , - . /
        ///   (0x3A)-(0x3F)=: ; &lt; = > ?
        ///   (0x5B)-(0x5E)=[ \ ] ^
        ///   (0x60)=`
        ///   (0x7B)-(0x7E)={ | } ~
        ///   (0x80)-(0xFE)=ANSEL characters above 127
        ///
        /// Any 8-bit ASCII character except control characters (0x00–0x1F), alphanum, space ( ), number
        /// sign (#), at sign (@), _ underscore, and the DEL character (0x7F).
        ///
        /// <a href="https://en.wikipedia.org/wiki/ANSEL">Wikepedia: ANSEL</a>
        /// </remarks>
        public static bool IsGedcomOtherChar(this char target)
        {
            return target == (char)0x21 || 
                   target == (char)0x22 ||
                   (target >= (char)0x24 && target <= (char)0x2F) ||
                   (target >= (char)0x3A && target <= (char)0x3F) ||
                   (target >= (char)0x5B && target <= (char)0x5E) ||
                   target == (char)0x60 || 
                   (target >= (char)0x7B && target <= (char)0x7E) ||
                   AnselAsUnicodeChars.Contains(target);
        }
        
        /// <summary>
        /// Determines if the target value is a GEDCOM non-at character.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <remarks>
        /// non_at:=
        ///   [alpha | digit | otherchar | (0x23) | (0x20 ) ]
        ///   where:
        ///   (0x20)=space character
        ///   (0x23)=#
        /// </remarks>
        public static bool IsGedcomNonAt(this char target)
        {
            return target.IsGedcomAlphanum() || target == Space || target == NumberSign || target.IsGedcomOtherChar();
        }
    }
}