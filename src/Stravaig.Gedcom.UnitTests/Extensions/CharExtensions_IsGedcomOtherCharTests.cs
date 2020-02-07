using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    public class CharExtensions_IsGedcomNonAtTests
    {
        [Test]
        [TestCaseSource(nameof(ValidAsciiChars))]
        public void IsGedcomNonAt_ForValidAsciiChars_ReturnsTrue(char asciiChar)
        {
            asciiChar.IsGedcomNonAt().ShouldBeTrue();
        }

        [Test]
        public void IsGedcomNonAt_WhenAtSymbol_ReturnsFalse()
        {
            '@'.IsGedcomNonAt().ShouldBeFalse();
        }
        
        [Test]
        [TestCaseSource(typeof(CharExtensions_Data), nameof(CharExtensions_Data.ValidAnselCharacters))]
        public void IsGedcomNonAt_ForValidAnselChars_ReturnsTrue(char anselChar)
        {
            anselChar.IsGedcomNonAt().ShouldBeTrue();
        }

        
        private static IEnumerable<char> ValidAsciiChars()
        {
            // All ASCII display characters are valid except the at symbol.
            for (char c = (char) 0x20; c <= (char) 0x7E; c++)
            {
                if (c == '@')
                    continue;
                yield return c;
            }
        }
    }
    
    [TestFixture]
    public class CharExtensions_IsGedcomOtherCharTests
    {
        [Test]
        [TestCaseSource(nameof(ValidSymbols))]
        public void IsGedcomOtherChar_ForValidSymbols_ReturnsTrue(char symbolChar)
        {
            symbolChar.IsGedcomOtherChar().ShouldBeTrue();
        }
        
        [Test]
        [TestCaseSource(typeof(CharExtensions_Data), nameof(CharExtensions_Data.ValidAnselCharacters))]
        public void IsGedcomOtherChar_ForValidAnselCharacters_ReturnsTrue(char alphaChar)
        {
            alphaChar.IsGedcomOtherChar().ShouldBeTrue();
        }
        
        private static IEnumerable<char> ValidSymbols()
        {
            return "!\"$%&'()*+,-./:;<=>?[\\]^`{|}~";
        }
    }
}