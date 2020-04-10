using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.UnitTests._helpers;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomDatabaseTests : AsyncTestBase
    {
        [Test]
        public void AddRecord_NullRecord_ThrowsException()
        {
            GedcomDatabase database = new GedcomDatabase();
            Should.Throw<ArgumentNullException>(() => database.AddRecord(null));
        }

        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task Populate_NullReader_ThrowsException(Read type)
        {
            GedcomDatabase database = new GedcomDatabase();
            try
            {
                await MaybePopulateAsync(type, database, null);
                Assert.Fail("Should have thrown an ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                // This is fine - the test worked
            }
        }

        private async Task MaybePopulateAsync(Read type, GedcomDatabase database, GedcomRecordReader reader)
        {
            if (type == Read.Synchronous)
                database.Populate(reader);
            else
                await database.PopulateAsync(reader);
        }
    }
}