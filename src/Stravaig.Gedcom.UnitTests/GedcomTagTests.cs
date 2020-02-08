using System;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomTagTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("not-a-Tag")]
        [TestCase("@I_AM_A_POINTER@")]
        public void ctor_InvalidValue_ThrowsException(string value)
        {
            Should.Throw<ArgumentException>(() => new GedcomTag(value));
        }

        [Test]
        public void ctor_ValidValue_CreatesTag()
        {
            string TagString = "I_AM_A_TAG";
            GedcomTag p = new GedcomTag(TagString);
            p.ToString().ShouldBe(TagString);
        }
        
        [Test]
        public void ctor_AnotherTag_CreatesNewEqualTag()
        {
            string TagString = "I_AM_A_TAG";
            GedcomTag p1 = new GedcomTag(TagString);
            GedcomTag p2 = new GedcomTag(p1);

            p1.ToString().ShouldBe(TagString);
            p2.ToString().ShouldBe(TagString);
            p1.Equals(p2).ShouldBeTrue();
        }
        
        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            GedcomTag p1 = new GedcomTag("I_AM_A_TAG");

            p1.Equals(null).ShouldBeFalse();
        }
        
        [Test]
        public void Equals_NotATag_ReturnsFalse()
        {
            GedcomTag p1 = new GedcomTag("I_AM_A_TAG");

            // ReSharper disable once SuspiciousTypeConversion.Global
            p1.Equals(123).ShouldBeFalse();
        }

        [Test]
        public void GetHashCode_WithEquivalentTag_ProducesSameHashCode()
        {
            string TagString = "I_AM_A_TAG";
            GedcomTag p1 = new GedcomTag(TagString);
            GedcomTag p2 = new GedcomTag(TagString);

            p1.GetHashCode().ShouldBe(p2.GetHashCode());
        }

        [Test]
        public void GetHashCode_WithNonEquivalentTag_ProducesDifferentHashCode()
        {
            GedcomTag p1 = new GedcomTag("FIRST_TAG");
            GedcomTag p2 = new GedcomTag("SECOND_TAG");

            p1.GetHashCode().ShouldNotBe(p2.GetHashCode());
        }

        [Test]
        public void OperatorEquals_WithEquivalentTag_ReturnsTrue()
        {
            string TagString = "I_AM_A_TAG";
            GedcomTag p1 = new GedcomTag(TagString);
            GedcomTag p2 = new GedcomTag(TagString);

            (p1 == p2).ShouldBeTrue();
        }
        
        [Test]
        public void OperatorNotEquals_WithEquivalentTag_ReturnsFalse()
        {
            string TagString = "I_AM_A_TAG";
            GedcomTag p1 = new GedcomTag(TagString);
            GedcomTag p2 = new GedcomTag(TagString);

            (p1 != p2).ShouldBeFalse();
        }
        
        [Test]
        public void OperatorEquals_WithDifferentTags_ReturnsFalse()
        {
            GedcomTag p1 = new GedcomTag("TAG_A");
            GedcomTag p2 = new GedcomTag("TAB_B");

            (p1 == p2).ShouldBeFalse();
        }
        
        [Test]
        public void OperatorNotEquals_WithDifferentTags_ReturnsTrue()
        {
            GedcomTag p1 = new GedcomTag("TAG_C");
            GedcomTag p2 = new GedcomTag("TAG_D");

            (p1 != p2).ShouldBeTrue();
        }
    }
}