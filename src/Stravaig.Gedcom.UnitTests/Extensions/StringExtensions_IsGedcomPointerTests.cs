using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    public partial class StringExtensionsTests
    {
        [TestFixture]
        public class IsGedcomPointerTests
        {

            [Test]
            [TestCase(null, Description = "Null is not a string.")]
            [TestCase("", Description = "Empty string is not a pointer.")]
            [TestCase("@", Description = "Start of Pointer character alone is not a pointer.")]
            [TestCase("@@", Description = "Pointer bookends alone are not a pointer.")]
            [TestCase("@$cant_start_with_$@", Description = "Underscore is the only starting symbol allowed.")]
            [TestCase("@123456789012345678901@", Description = "Pointer is too long.")]
            public void FailingValues_ReturnFalse(string target)
            {
                target.IsGedcomPointer().ShouldBeFalse();
            }

            [Test]
            [TestCase("@a@", Description = "Minimal passing pointer.")]
            [TestCase("@12345678901234567890@", Description = "Max length pointer.")]
            [TestCase("@_!$%&'()*+,-/;:<^>`~@", Description = "Pointer made of symbols.")]
            public void PassingValues_ReturnTrue(string target)
            {
                target.IsGedcomPointer().ShouldBeTrue();
            }
        }
    }
}