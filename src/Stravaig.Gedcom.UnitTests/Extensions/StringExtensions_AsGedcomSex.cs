using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    public class StringExtensions_AsGedcomSex
    {
        [Test]
        public void M_IsMale()
        {
            "M".AsGedcomSex().ShouldBe(GedcomSex.Male);
        }

        [Test]
        public void F_IsFemale()
        {
            "F".AsGedcomSex().ShouldBe(GedcomSex.Female);
        }

        [Test]
        public void U_IsUndetermined()
        {
            "U".AsGedcomSex().ShouldBe(GedcomSex.Undetermined);
        }

        [Test]
        public void EmptyString_IsNotKnown()
        {
            string.Empty.AsGedcomSex().ShouldBe(GedcomSex.NotKnown);
        }
        
        [Test]
        public void Null_IsNotKnown()
        {
            ((string)null).AsGedcomSex().ShouldBe(GedcomSex.NotKnown);
        }
    }
}