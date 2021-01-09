using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    public partial class StringExtensionsTests
    {
        public class SectionTests
        {
            [Test]
            public void NullString_ReturnsEmptyString()
            {
                string str = null;
                // ReSharper disable once ExpressionIsAlwaysNull
                str.Section(0, 0).ShouldBeEmpty();
            }

            [Test]
            public void EmptyString_ReturnsEmptyString()
            {
                string.Empty.Section(0, 0).ShouldBeEmpty();
            }

            [Test]
            [TestCase("ABCDEF", 0, 0, ExpectedResult = "A")]
            [TestCase("ABCDEF", 0, 1, ExpectedResult = "AB")]
            [TestCase("ABCDEF", 2, 2, ExpectedResult = "C")]
            [TestCase("ABCDEF", 5, 5, ExpectedResult = "F")]
            [TestCase("ABCDEF", 5, 6, ExpectedResult = "F")]
            [TestCase("ABCDEF", 4, 7, ExpectedResult = "EF")]
            [TestCase("ABCDEF", 6, 6, ExpectedResult = "")]
            public string HappyPaths(string input, int startIndex, int endIndex)
            {
                return input.Section(startIndex, endIndex);
            }
        }
    }
}