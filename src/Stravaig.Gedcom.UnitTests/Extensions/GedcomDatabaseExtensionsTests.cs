using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;
using Stravaig.Gedcom.UnitTests._helpers;

namespace Stravaig.Gedcom.UnitTests.Extensions
{
    [TestFixture]
    public class GedcomDatabaseExtensionsTests
    {
        [Test]
        public void LoadFromTextReader()
        {
            using TextReader reader = ResourceFactory.GetReader(this.GetType(), "Relationships.ged");
            GedcomDatabase db = new GedcomDatabase();
            db.Records.Count.ShouldBe(0);
            db.Populate(reader);
            db.Records.Count.ShouldBeGreaterThan(0);
        }
        
        [Test]
        public async Task LoadFromTextReaderAsync()
        {
            using TextReader reader = ResourceFactory.GetReader(this.GetType(), "Relationships.ged");
            GedcomDatabase db = new GedcomDatabase();
            db.Records.Count.ShouldBe(0);
            await db.PopulateAsync(reader);
            db.Records.Count.ShouldBeGreaterThan(0);
        }

        [Test]
        public void LoadFromStream()
        {
            using Stream reader = ResourceFactory.GetStream(this.GetType(), "Relationships.ged");
            GedcomDatabase db = new GedcomDatabase();
            db.Records.Count.ShouldBe(0);
            db.Populate(reader);
            db.Records.Count.ShouldBeGreaterThan(0);
        }
        
        [Test]
        public async Task LoadFromStreamAsync()
        {
            using Stream reader = ResourceFactory.GetStream(this.GetType(), "Relationships.ged");
            GedcomDatabase db = new GedcomDatabase();
            db.Records.Count.ShouldBe(0);
            await db.PopulateAsync(reader);
            db.Records.Count.ShouldBeGreaterThan(0);
        }

        [Test]
        public void LoadFromFile()
        {
            string fileName = ResourceFactory.GetFileName(this.GetType(), "Relationships.ged");
            GedcomDatabase db = new GedcomDatabase();
            db.Records.Count.ShouldBe(0);
            db.PopulateFromFile(fileName);
            db.Records.Count.ShouldBeGreaterThan(0);
        }

        [Test]
        public async Task LoadFromFileAsync()
        {
            string fileName = ResourceFactory.GetFileName(this.GetType(), "Relationships.ged");
            GedcomDatabase db = new GedcomDatabase();
            db.Records.Count.ShouldBe(0);
            await db.PopulateFromFileAsync(fileName);
            db.Records.Count.ShouldBeGreaterThan(0);
        }
    }
}