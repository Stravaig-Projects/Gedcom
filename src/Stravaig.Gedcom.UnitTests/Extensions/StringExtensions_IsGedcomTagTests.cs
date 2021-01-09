using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    public partial class StringExtensionsTests
    {
        [TestFixture]
        public class IsGedcomTagTests
        {
            [Test]
            [TestCase(null, Description = "Null is not a tag.")]
            [TestCase("", Description = "Empty string is not a tag.")]
            [TestCase("12345678901234567890123456789012", Description = "Tag is too long.")]
            public void FailingValues_ReturnFalse(string target)
            {
                target.IsGedcomTag().ShouldBeFalse();
            }

            [Test]
            [TestCase("a", Description = "Minimal passing tag.")]
            [TestCase("1234567890123456789012345678901", Description = "Max length tag.")]
            [TestCase("1234567890", Description = "Tag made of numbers.")]
            [TestCase("abcdefghijklmnopqrstuvwxyz", Description = "Tag made of letters.")]
            [TestCase("_", Description = "Tag made of underscore.")]
            public void PassingValues_ReturnTrue(string target)
            {
                target.IsGedcomTag().ShouldBeTrue();
            }
        }
    }
}