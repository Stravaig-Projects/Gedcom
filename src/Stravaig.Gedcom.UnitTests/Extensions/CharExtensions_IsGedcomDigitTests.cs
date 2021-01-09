using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    public partial class CharExtensionsTests
    {
        public class IsGedcomDigitTests
        {
            [Test]
            [TestCaseSource(nameof(ValidNumbers))]
            public void IsGedcomDigit_ForValidNumber_ReturnsTrue(char number)
            {
                number.IsGedcomDigit().ShouldBeTrue();
            }

            [Test]
            [TestCaseSource(nameof(OtherNumbers))]
            public void IsGedcomDigit_ForOtherNumber_ReturnsFalse(char number)
            {
                number.IsGedcomDigit().ShouldBeFalse();
            }

            [Test]
            [TestCaseSource(nameof(Letters))]
            public void IsGedcomDigit_ForLetters_ReturnsFalse(char letter)
            {
                letter.IsGedcomDigit().ShouldBeFalse();
            }

            private static IEnumerable<char> ValidNumbers()
            {
                for (char c = '0'; c <= '9'; c++)
                    yield return c;
            }

            private static IEnumerable<char> OtherNumbers()
            {
                for (char c = '⁰'; c <= '⁹'; c++)
                    yield return c;
            }

            private static IEnumerable<char> Letters()
            {
                for (char c = 'a'; c <= 'z'; c++)
                    yield return c;
                for (char c = 'A'; c <= 'Z'; c++)
                    yield return c;
            }
        }
    }
}