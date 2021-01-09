using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public partial class CharExtensionsTests
    {
        public class IsGedcomAlphaTests
        {
            [Test]
            [TestCaseSource(nameof(ValidAlphaCharacters))]
            public void IsGedcomAlpha_ForValidCharacters_ReturnsTrue(char alphaChar)
            {
                alphaChar.IsGedcomAlpha().ShouldBeTrue();
            }

            [Test]
            [TestCaseSource(nameof(Numbers))]
            public void IsGedcomAlpha_ForNumbers_ReturnsFalse(char alphaChar)
            {
                alphaChar.IsGedcomAlpha().ShouldBeFalse();
            }

            [Test]
            [TestCaseSource(nameof(Symbols))]
            public void IsGedcomAlpha_ForSymbols_ReturnsFalse(char alphaChar)
            {
                alphaChar.IsGedcomAlpha().ShouldBeFalse();
            }

            [Test]
            [TestCaseSource(nameof(AccentedLetters))]
            public void IsGedcomAlpha_ForAccentedLetters_ReturnsFalse(char alphaChar)
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

            private static IEnumerable<char> Numbers()
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
}