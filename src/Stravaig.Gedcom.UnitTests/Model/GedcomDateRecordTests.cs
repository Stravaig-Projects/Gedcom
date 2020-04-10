using System;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.UnitTests.Model
{
    [TestFixture]
    public class GedcomDateRecordTests
    {
        private GedcomDatabase _database;

        [SetUp]
        public void SetUp()
        {
            _database = new GedcomDatabase();
        }
        
        [Test]
        public void ctor_NullRecord_ThrowsException()
        {
            Should.Throw<ArgumentNullException>(() => new GedcomDateRecord(null, _database));
        }

        [Test]
        public void ctor_NullDatabase_ThrowsException()
        {
            var dodgyRecord = new GedcomRecord("0 DATE 08 APR 2020".AsGedcomLine());
            Should.Throw<ArgumentException>(() => new GedcomDateRecord(dodgyRecord, null));
        }

        [Test]
        public void ctor_NotDateRecord_ThrowsException()
        {
            var dodgyRecord = new GedcomRecord("0 BAD This is not a date record".AsGedcomLine());
            Should.Throw<ArgumentException>(() => new GedcomDateRecord(dodgyRecord, _database));
        }

        [Test]
        public void ctor_RawDateRecord_CreatesDateRecord()
        {
            var dateRecord = GetDate("0 DATE 01 APR 2020");
            dateRecord.RawDateValue.ShouldBe("01 APR 2020");
        }

        private GedcomDateRecord GetDate(string lineText)
        {
            var record = new GedcomRecord(lineText.AsGedcomLine());
            var dateRecord = new GedcomDateRecord(record, _database);
            return dateRecord;
        }
    }
}