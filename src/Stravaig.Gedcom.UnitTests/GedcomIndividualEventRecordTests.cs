using System;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomIndividualEventRecordTests
    {
        [Test]
        public void ctor_NullRecord_ThrowsException()
        {
            var message = Should.Throw<ArgumentNullException>(() => new GedcomIndividualEventRecord(null))
                .Message;
            Console.WriteLine(message);
        }
        
        [Test]
        public void ctor_NotEventRecord_ThrowsException()
        {
            var line = new GedcomLine(1.AsGedcomLevel(), "NOTEVENT".AsGedcomTag());
            var record = new GedcomRecord(line);
            var message = Should.Throw<ArgumentException>(() => new GedcomIndividualEventRecord(record))
                .Message;
            Console.WriteLine(message);
        }
    }
}