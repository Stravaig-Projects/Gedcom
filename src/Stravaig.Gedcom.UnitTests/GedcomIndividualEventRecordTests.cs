using System;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomIndividualEventRecordTests
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
            var message = Should.Throw<ArgumentNullException>(() => new GedcomIndividualEventRecord(null, _database))
                .Message;
            Console.WriteLine(message);
        }
        
        [Test]
        public void ctor_NullDatabase_ThrowsException()
        {
            var line = new GedcomLine(1.AsGedcomLevel(), "DATE".AsGedcomTag(), "08 APR 2020");
            var record = new GedcomRecord(line);
            var message = Should.Throw<ArgumentNullException>(() => new GedcomIndividualEventRecord(record, null))
                .Message;
            Console.WriteLine(message);
        }

        
        [Test]
        public void ctor_NotEventRecord_ThrowsException()
        {
            var line = new GedcomLine(1.AsGedcomLevel(), "NOTEVENT".AsGedcomTag());
            var record = new GedcomRecord(line);
            var message = Should.Throw<ArgumentException>(() => new GedcomIndividualEventRecord(record, _database))
                .Message;
            Console.WriteLine(message);
        }
    }
}