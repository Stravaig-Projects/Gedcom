using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomLevelTests
    {
        [Test]
        [TestCaseSource(nameof(ValidLevels))]
        public void ctor_ValidValues_CreateLevelObject(int value)
        {
            _ = new GedcomLevel(value);
        }

        [Test]
        [TestCaseSource(nameof(ValidLevelStrings))]
        public void ctor_ValidStringValues_CreateLevelObject(string value)
        {
            _ = new GedcomLevel(value);
        }
        
        [Test]
        public void ctor_InvalidStringLevel_ThrowArgumentException()
        {
            Should.Throw<ArgumentException>( () => new GedcomLevel("123"));
        }
        
        [Test]
        public void ctor_AnotherGedcomLevel_CreateDuplicateObject()
        {
            GedcomLevel l1 = new GedcomLevel(3);
            GedcomLevel l2 = new GedcomLevel(l1);
            
            l1.ShouldBe(l2);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void ctor_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            Should.Throw<ArgumentOutOfRangeException>(() => new GedcomLevel(value));
        }

        [Test]
        public void ToString_ReturnsStringRepresentationOfTheInternalValue()
        {
            GedcomLevel level = new GedcomLevel(2);
            level.ToString().ShouldBe("2");
        }

        [Test]
        public void GetHashCode_ReturnsInternalValue()
        {
            GedcomLevel level = new GedcomLevel(7);
            level.GetHashCode().ShouldBe(7);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            GedcomLevel level = new GedcomLevel(11);
            level.Equals(null).ShouldBeFalse();
        }
        
        [Test]
        public void Equals_WithOtherType_ReturnsFalse()
        {
            GedcomLevel level = new GedcomLevel(11);
            // ReSharper disable once SuspiciousTypeConversion.Global
            level.Equals(true).ShouldBeFalse();
        }
        
        [Test]
        public void Equals_WithOtherGedcomLevel_ReturnsFalse()
        {
            GedcomLevel l1 = new GedcomLevel(11);
            GedcomLevel l2 = new GedcomLevel(13);
            l1.Equals(l2).ShouldBeFalse();
        }

        [Test]
        public void Equals_WithEquivalentGedcomLevel_ReturnsTrue()
        {
            GedcomLevel l1 = new GedcomLevel(17);
            GedcomLevel l2 = new GedcomLevel(17);
            l1.Equals(l2).ShouldBeTrue();
        }
        
        [Test]
        public void OperatorEquals_WithEquivalentLevel_ReturnsTrue()
        {
            GedcomLevel p1 = new GedcomLevel(1);
            GedcomLevel p2 = new GedcomLevel(1);

            (p1 == p2).ShouldBeTrue();
        }
        
        [Test]
        public void OperatorNotEquals_WithEquivalentLevel_ReturnsFalse()
        {
            GedcomLevel p1 = new GedcomLevel(1);
            GedcomLevel p2 = new GedcomLevel(1);

            (p1 != p2).ShouldBeFalse();
        }
        
        [Test]
        public void OperatorEquals_WithDifferentLevels_ReturnsFalse()
        {
            GedcomLevel p1 = new GedcomLevel(2);
            GedcomLevel p2 = new GedcomLevel(3);

            (p1 == p2).ShouldBeFalse();
        }
        
        [Test]
        public void OperatorNotEquals_WithDifferentLevels_ReturnsTrue()
        {
            GedcomLevel p1 = new GedcomLevel(4);
            GedcomLevel p2 = new GedcomLevel(5);

            (p1 != p2).ShouldBeTrue();
        }

        [Test]
        [TestCase(0,0)]
        [TestCase(0,1)]
        [TestCase(1,1)]
        [TestCase(1,0)]
        [TestCase(2,0)]
        public void CanFollowFrom_ValidAssociations_ReturnTrue(int first, int second)
        {
            GedcomLevel firstLevel = new GedcomLevel(first);
            GedcomLevel secondLevel = new GedcomLevel(second);

            secondLevel.CanFollowFrom(firstLevel).ShouldBeTrue();
        }
        
        [Test]
        [TestCase(0,2)]
        [TestCase(1,3)]
        public void CanFollowFrom_InvalidAssociations_ReturnFalse(int first, int second)
        {
            GedcomLevel firstLevel = new GedcomLevel(first);
            GedcomLevel secondLevel = new GedcomLevel(second);

            secondLevel.CanFollowFrom(firstLevel).ShouldBeFalse();
        }

        [Test]
        [TestCase(0,0)]
        [TestCase(0,1)]
        [TestCase(1,1)]
        [TestCase(1,0)]
        [TestCase(2,0)]
        public void CanBeFollowedBy_ValidAssociations_ReturnTrue(int first, int second)
        {
            GedcomLevel firstLevel = new GedcomLevel(first);
            GedcomLevel secondLevel = new GedcomLevel(second);

            firstLevel.CanBeFollowedBy(secondLevel).ShouldBeTrue();
        }
        
        [Test]
        [TestCase(0,2)]
        [TestCase(1,3)]
        public void CanBeFollowedBy_InvalidAssociations_ReturnFalse(int first, int second)
        {
            GedcomLevel firstLevel = new GedcomLevel(first);
            GedcomLevel secondLevel = new GedcomLevel(second);

            firstLevel.CanBeFollowedBy(secondLevel).ShouldBeFalse();
        }

        [Test]
        [TestCaseSource(nameof(ValidNextLineLevelMaxValues))]
        public void NextLineLevelMax_ForValuesBelow98_ReturnValuePlusOne(GedcomLevel level, GedcomLevel max)
        {
            level.NextLineLevelMax.ShouldBe(max);
        }

        [Test]
        [TestCaseSource(nameof(ValidSubordinateLevels))]
        public void IsSubordinateTo_ForValidPairs_ReturnsTrue(GedcomLevel parentLevel, GedcomLevel subordinateLevel)
        {
            subordinateLevel.IsSubordinateTo(parentLevel)
                .ShouldBeTrue();
        }

        [Test]
        [TestCaseSource(nameof(InvalidSubordinateLevels))]
        public void IsSubordinateTo_ForInvalidPairs_ReturnsFalse(GedcomLevel parentLevel, GedcomLevel subordinateLevel)
        {
            subordinateLevel.IsSubordinateTo(parentLevel)
                .ShouldBeFalse();
        }


        
        private static IEnumerable<GedcomLevel[]> InvalidSubordinateLevels()
        {
            for (int i = 0; i <= 99; i++)
                yield return new [] {i.AsGedcomLevel() , i.AsGedcomLevel()};
            for (int i = 0; i <= 98; i++)
                yield return new [] {(i + 1).AsGedcomLevel() , i.AsGedcomLevel()};
        }
        private static IEnumerable<GedcomLevel[]> ValidSubordinateLevels()
        {
            for (int i = 0; i <= 98; i++)
                yield return new [] {i.AsGedcomLevel(), (i + 1).AsGedcomLevel()};
        }
        
        private static IEnumerable<GedcomLevel[]> ValidNextLineLevelMaxValues()
        {
            for (int i = 0; i <= 98; i++)
                yield return new [] {i.AsGedcomLevel(), (i + 1).AsGedcomLevel()};
        }
        
        private static IEnumerable<int> ValidLevels()
        {
            for (int i = 0; i <= 99; i++)
                yield return i;
        }
        
        private static IEnumerable<string> ValidLevelStrings()
        {
            return ValidLevels()
                .Select(l => l.ToString(CultureInfo.InvariantCulture));
        }
    }
}
