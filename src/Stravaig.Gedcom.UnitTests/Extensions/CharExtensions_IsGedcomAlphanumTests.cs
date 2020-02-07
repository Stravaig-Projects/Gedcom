using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    public class CharExtensions_IsGedcomAlphanumTests
    {
        [Test]
        [TestCaseSource(nameof(ValidAlphaCharacters))]
        public void IsGedcomAlphanum_ForValidCharacters_ReturnsTrue(char alphaChar)
        {
            alphaChar.IsGedcomAlpha().ShouldBeTrue();
        }
        
        [Test]
        [TestCaseSource(nameof(ValidNumbers))]
        public void IsGedcomAlphanum_ForValidNumber_ReturnsTrue(char number)
        {
            number.IsGedcomDigit().ShouldBeTrue();
        }
        
        [Test]
        [TestCaseSource(nameof(Symbols))]
        public void IsGedcomAlphanum_ForSymbols_ReturnsFalse(char alphaChar)
        {
            alphaChar.IsGedcomAlpha().ShouldBeFalse();
        }
        
        [Test]
        [TestCaseSource(nameof(AccentedLetters))]
        public void IsGedcomAlphanum_ForAccentedLetters_ReturnsFalse(char alphaChar)
        {
            alphaChar.IsGedcomAlpha().ShouldBeFalse();
        }
        
        private static IEnumerable<char> ValidAlphaCharacters()
        {
            for (char c = 'a'; c <= 'z'; c++)
                yield return c;
            for (char c = 'A'; c <= 'Z'; c++)
                yield return c;
            yield return '_';
        }

        private static IEnumerable<char> ValidNumbers()
        {
            for (char c = '0'; c <= '9'; c++)
                yield return c;
        }
        
        private static IEnumerable<char> Symbols()
        {
            return "!£$%^&*()-+={}[]:@~;'#<>?,./\\|\"`¬";
        }

        private static IEnumerable<char> AccentedLetters()
        {
            for (char c = (char) 0xC0; c <= (char) 0xFF; c++)
                yield return c;
        }
    }
}