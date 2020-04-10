using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Constants;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Comparers;
using Stravaig.Gedcom.Settings;

namespace Stravaig.Gedcom.UnitTests.Model.Comparers
{
    [TestFixture]
    public class DateComparerTests
    {
        private GedcomDatabase _database;
        [SetUp]
        public void SetUp()
        {
            _database = new GedcomDatabase();
        }
        
        [Test]
        public void BothNull_ReturnsXEqualsY()
        {
            DateComparer.CompareDate(null, null).ShouldBe(Order.XEqualsY);
        }

        [Test]
        public void XIsIncoherentYIsIncoherent_Returns_XIsLessThanY()
        {
            var x = GetGedcomDate("(Yesterday)");
            var y = GetGedcomDate("(Today)");
            DateComparer.CompareDate(x, y).ShouldBe(Order.XEqualsY);
        }

        [Test]
        public void XIsNullYIsNot_Returns_XIsLessThanY()
        {
            var y = GetGedcomDate("09 APR 2020");
            DateComparer.CompareDate(null, y).ShouldBe(Order.XIsLessThanY);
        }

        [Test]
        public void XIsIncoherentYIsNot_Returns_XIsLessThanY()
        {
            var x = GetGedcomDate("(Today)");
            var y = GetGedcomDate("09 APR 2020");
            DateComparer.CompareDate(null, y).ShouldBe(Order.XIsLessThanY);
        }

        [Test]
        public void XIsNotNullYIsNull_Returns_XIsGreaterThanY()
        {
            var x = GetGedcomDate("09 APR 2020");
            DateComparer.CompareDate(x, null).ShouldBe(Order.XIsGreaterThanY);
        }
        
        [Test]
        public void XIsNotNullYIsIncoherent_Returns_XIsGreaterThanY()
        {
            var x = GetGedcomDate("09 APR 2020");
            var y = GetGedcomDate("(Today)");
            DateComparer.CompareDate(x, null).ShouldBe(Order.XIsGreaterThanY);
        }

        [Test]
        [TestCase("10 APR 2020", "10 APR 2020", ExpectedResult = Order.XEqualsY)]
        [TestCase("08 APR 2020", "09 APR 2020", ExpectedResult = Order.XIsLessThanY)]
        [TestCase("09 APR 2020", "08 APR 2020", ExpectedResult = Order.XIsGreaterThanY)]
        [TestCase("APR 2020", "APR 2020", ExpectedResult = Order.XEqualsY)]
        [TestCase("APR 2020", "MAY 2020", ExpectedResult = Order.XIsLessThanY)]
        [TestCase("JUN 2020", "MAY 2020", ExpectedResult = Order.XIsGreaterThanY)]
        [TestCase("2020", "2020", ExpectedResult = Order.XEqualsY)]
        [TestCase("2020", "MAY 2021", ExpectedResult = Order.XIsLessThanY)]
        [TestCase("2021", "2020", ExpectedResult = Order.XIsGreaterThanY)]
        [TestCase("10 MAR 2020", "MAR 2020", DateOrderingRule.BeginningOfExtent, ExpectedResult = Order.XIsGreaterThanY)]
        [TestCase("10 MAR 2020", "MAR 2020", DateOrderingRule.MiddleOfExtent, ExpectedResult = Order.XIsLessThanY)]
        [TestCase("10 MAR 2020", "MAR 2020", DateOrderingRule.EndOfExtent, ExpectedResult = Order.XIsLessThanY)]
        [TestCase("20 MAR 2020", "MAR 2020", DateOrderingRule.MiddleOfExtent, ExpectedResult = Order.XIsGreaterThanY)]
        [TestCase("20 MAR 2020", "MAR 2020", DateOrderingRule.EndOfExtent, ExpectedResult = Order.XIsLessThanY)]
        [TestCase("FROM 1 MAR 2020 TO 31 MAR 2020", "BET 1 MAR 2020 AND 31 MAR 2020", DateOrderingRule.BeginningOfExtent, ExpectedResult = Order.XEqualsY)]        
        [TestCase("FROM 1 MAR 2020 TO 31 MAR 2020", "BET 1 MAR 2020 AND 31 MAR 2020", DateOrderingRule.MiddleOfExtent, ExpectedResult = Order.XEqualsY)]        
        [TestCase("FROM 1 MAR 2020 TO 31 MAR 2020", "BET 1 MAR 2020 AND 31 MAR 2020", DateOrderingRule.EndOfExtent, ExpectedResult = Order.XEqualsY)]
        [TestCase("FROM 2010 TO 2020", "BET 2011 AND 2019", DateOrderingRule.BeginningOfExtent, ExpectedResult = Order.XIsLessThanY)]
        [TestCase("FROM 2010 TO 2020", "BET 2011 AND 2019", DateOrderingRule.EndOfExtent, ExpectedResult = Order.XIsGreaterThanY)]
        public int HappyPath(string rawX, string rawY, DateOrderingRule rule = DateOrderingRule.BeginningOfExtent)
        {
            _database.Settings.DateOrderingRule = rule;
            var x = GetGedcomDate(rawX);
            var y = GetGedcomDate(rawY);
            var result = DateComparer.CompareDate(x, y);
            return result;
        }

        private GedcomDateRecord GetGedcomDate(string rawDateValue)
        {
            var line = $"0 DATE {rawDateValue}".AsGedcomLine();
            GedcomRecord record = GedcomRecord.From(line, null);
            GedcomDateRecord result = new GedcomDateRecord(record, _database);
            return result;
        }
    }
}