using NUnit.Framework;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Tests.Services
{
    [TestFixture]
    public class DateRendererTests
    {
        private GedcomDatabase _database;
        private DateRenderer _renderer;

        [SetUp]
        public void SetUp()
        {
            _database = new GedcomDatabase();
            _renderer = new DateRenderer();
        }
        
        [Test]
        [TestCase("11 APR 2020", ExpectedResult = "on 11th of April 2020")]
        [TestCase("1 FEB 1985", ExpectedResult = "on 1st of February 1985")]
        [TestCase("2 JAN 1999", ExpectedResult = "on 2nd of January 1999")]
        [TestCase("3 MAR 1977", ExpectedResult = "on 3rd of March 1977")]
        [TestCase("MAY 1974", ExpectedResult = "in May 1974")]
        [TestCase("1969", ExpectedResult = "in 1969")]
        public string HappyPaths(string date)
        {
            var record = GetDateRecord(date);
            var result = _renderer.RenderAsProse(record);
            return result;
        }

        private GedcomDateRecord GetDateRecord(string date)
        {
            var line = $"0 DATE {date}".AsGedcomLine();
            var record = new GedcomRecord(line);
            var result = new GedcomDateRecord(record, _database);
            return result;
        }
    }
}