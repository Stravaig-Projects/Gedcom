using System.Collections.Generic;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    public class CharExtensions_Data
    {
        public static IEnumerable<char> ValidAnselCharacters()
        {
            // A0 - AF range
            yield return (char)0x0141; // Ł 
            yield return (char)0x00D8; // Ø
            yield return (char)0x0110; // Đ
            yield return (char)0x00DE; // Þ
            yield return (char)0x00C6; // Æ
            yield return (char)0x0152; // Œ
            yield return (char)0x02B9; // ʹ
            yield return (char)0x00B7; // ·
            yield return (char)0x226D; // ♭
            yield return (char)0x00AE; // ®
            yield return (char)0x00B1; // ± 
            yield return (char)0x01A0; // Ơ
            yield return (char)0x01AF; // Ư
            yield return (char)0x02BC; // ʼ

            yield return (char)0x02BB; // ʻ
            yield return (char)0x0142; // ł
            yield return (char)0x00F8; // ø
            yield return (char)0x0111; // đ
            yield return (char)0x00FE; // þ
            yield return (char)0x00E6; // æ
            yield return (char)0x0153; // œ
            yield return (char)0x02BA; // ʺ
            yield return (char)0x0131; // ı
            yield return (char)0x00A3; // £
            yield return (char)0x00F0; // ð
            yield return (char)0x01A1; // ơ
            yield return (char)0x01B0; // ư
            
            yield return (char)0x00B0; // °
            yield return (char)0x2113; // ℓ
            yield return (char)0x2117; // ℗
            yield return (char)0x00A9; // ©
            yield return (char)0x266F; // ♯
            yield return (char)0x00BF; // ¿
            yield return (char)0x00A1; // ¡
            
            yield return (char)0x0309; // ̉ 
            yield return (char)0x0300; //  ̀ 
            yield return (char)0x0301; //  ́ 
            yield return (char)0x0302; //  ̂ 
            yield return (char)0x0303; //  ̃ 
            yield return (char)0x0304; //  ̄ 
            yield return (char)0x0306; //  ̆ 
            yield return (char)0x0307; //  ̇ 
            yield return (char)0x0308; //  ̈ 
            yield return (char)0x030C; //  ̌ 
            yield return (char)0x030A; //  ̊ 
            yield return (char)0xFE20; // ︠ 
            yield return (char)0xFE21; // ︡ 
            yield return (char)0x0315; // ̕ 
            yield return (char)0x030B; //  ̋ 
            yield return (char)0x0310; // ̐ 
            
            yield return (char)0x0327; //  ̧ 
            yield return (char)0x0328; // ̨ 
            yield return (char)0x0323; // ̣ 
            yield return (char)0x0324; // ̤ 
            yield return (char)0x0325; // ̥ 
            yield return (char)0x0333; // ̳ 
            yield return (char)0x0332; // ̲ 
            yield return (char)0x0326; // ̦ 
            yield return (char)0x031C; // ̜ 
            yield return (char)0x032E; // ̮ 
            yield return (char)0xFE22; // ︢ 
            yield return (char)0xFE23; // ︣ 
            yield return (char)0x0313; //  ̓ 
            
            yield return (char)0x25A1; // □ empty box
            yield return (char)0x25A0; // ■ black box
            yield return (char)0x0065; // e midline e
            yield return (char)0x006F; // o midline o
            yield return (char)0x00Df; // ß es zet
            yield return (char)0x0338; // ̸  diacritic slash through char
        }

    }
}