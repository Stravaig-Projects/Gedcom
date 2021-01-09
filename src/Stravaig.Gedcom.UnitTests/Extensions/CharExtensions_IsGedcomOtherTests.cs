using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    public partial class CharExtensionsTests
    {
        [TestFixture]
        public class IsGedcomOtherCharTests
        {
            [Test]
            [TestCaseSource(nameof(ValidSymbols))]
            public void IsGedcomOtherChar_ForValidSymbols_ReturnsTrue(char symbolChar)
            {
                symbolChar.IsGedcomOtherChar().ShouldBeTrue();
            }

            [Test]
            [TestCaseSource(typeof(Data), nameof(Data.ValidAnselCharacters))]
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
}