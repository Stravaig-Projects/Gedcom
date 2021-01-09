using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    public partial class StringExtensionsTests
    {
        [TestFixture]
        public class IsGedcomLevelTests
        {
            [Test]
            [TestCase(null, Description = "Null is not a level.")]
            [TestCase("", Description = "Empty string is not a level.")]
            [TestCase(" ", Description = "Whitespace string is not a level.")]
            [TestCase("123", Description = "Level is too long.")]
            [TestCase("00", Description = "Level cannot have leading zero.")]
            [TestCase("01", Description = "Level cannot have leading zero.")]
            [TestCase("02", Description = "Level cannot have leading zero.")]
            [TestCase("03", Description = "Level cannot have leading zero.")]
            [TestCase("04", Description = "Level cannot have leading zero.")]
            [TestCase("05", Description = "Level cannot have leading zero.")]
            [TestCase("06", Description = "Level cannot have leading zero.")]
            [TestCase("07", Description = "Level cannot have leading zero.")]
            [TestCase("08", Description = "Level cannot have leading zero.")]
            [TestCase("09", Description = "Level cannot have leading zero.")]
            public void FailingValues_ReturnFalse(string target)
            {
                target.IsGedcomLevel().ShouldBeFalse();
            }

            [Test]
            [TestCaseSource(nameof(AllValidLevels))]
            public void PassingValues_ReturnTrue(string target)
            {
                target.IsGedcomLevel().ShouldBeTrue();
            }

            private static IEnumerable<string> AllValidLevels()
            {
                for (int i = 0; i <= 99; i++)
                    yield return i.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}