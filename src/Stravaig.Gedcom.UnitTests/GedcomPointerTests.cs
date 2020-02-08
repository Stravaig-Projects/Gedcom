using System;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomPointerTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("@@")]
        [TestCase("not-a-pointer")]
        public void ctor_InvalidValue_ThrowsException(string value)
        {
            Should.Throw<ArgumentException>(() => new GedcomPointer(value));
        }

        [Test]
        public void ctor_ValidValue_CreatesPointer()
        {
            string pointerString = "@I_AM_A_POINTER@";
            GedcomPointer p = new GedcomPointer(pointerString);
            p.ToString().ShouldBe(pointerString);
        }
        
        [Test]
        public void ctor_AnotherPointer_CreatesNewEqualPointer()
        {
            string pointerString = "@I_AM_A_POINTER@";
            GedcomPointer p1 = new GedcomPointer(pointerString);
            GedcomPointer p2 = new GedcomPointer(p1);

            p1.ToString().ShouldBe(pointerString);
            p2.ToString().ShouldBe(pointerString);
            p1.Equals(p2).ShouldBeTrue();
        }
        
        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            GedcomPointer p1 = new GedcomPointer("@I_AM_A_POINTER@");

            p1.Equals(null).ShouldBeFalse();
        }
        
        [Test]
        public void Equals_NotAPointer_ReturnsFalse()
        {
            GedcomPointer p1 = new GedcomPointer("@I_AM_A_POINTER@");

            // ReSharper disable once SuspiciousTypeConversion.Global
            p1.Equals(123).ShouldBeFalse();
        }

        [Test]
        public void GetHashCode_WithEquivalentPointer_ProducesSameHashCode()
        {
            string pointerString = "@I_AM_A_POINTER@";
            GedcomPointer p1 = new GedcomPointer(pointerString);
            GedcomPointer p2 = new GedcomPointer(pointerString);

            p1.GetHashCode().ShouldBe(p2.GetHashCode());
        }

        [Test]
        public void GetHashCode_WithNonEquivalentPointer_ProducesDifferentHashCode()
        {
            GedcomPointer p1 = new GedcomPointer("@FIRST_POINTER@");
            GedcomPointer p2 = new GedcomPointer("@SECOND_POINTER@");

            p1.GetHashCode().ShouldNotBe(p2.GetHashCode());
        }

        [Test]
        public void OperatorEquals_WithEquivalentPointer_ReturnsTrue()
        {
            string pointerString = "@I_AM_A_POINTER@";
            GedcomPointer p1 = new GedcomPointer(pointerString);
            GedcomPointer p2 = new GedcomPointer(pointerString);

            (p1 == p2).ShouldBeTrue();
        }
        
        [Test]
        public void OperatorNotEquals_WithEquivalentPointer_ReturnsFalse()
        {
            string pointerString = "@I_AM_A_POINTER@";
            GedcomPointer p1 = new GedcomPointer(pointerString);
            GedcomPointer p2 = new GedcomPointer(pointerString);

            (p1 != p2).ShouldBeFalse();
        }
        
        [Test]
        public void OperatorEquals_WithDifferentPointers_ReturnsFalse()
        {
            GedcomPointer p1 = new GedcomPointer("@Pointer_A@");
            GedcomPointer p2 = new GedcomPointer("@Pointer_B@");

            (p1 == p2).ShouldBeFalse();
        }
        
        [Test]
        public void OperatorNotEquals_WithDifferentPointers_ReturnsTrue()
        {
            GedcomPointer p1 = new GedcomPointer("@Pointer_C@");
            GedcomPointer p2 = new GedcomPointer("@Pointer_D@");

            (p1 != p2).ShouldBeTrue();
        }
    }
}