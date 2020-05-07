using System.IO;
using NUnit.Framework;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.UnitTests._helpers;

namespace Stravaig.Gedcom.UnitTests.Model.Extensions
{
    [TestFixture]
    public class RelationshipExtensionsTests
    {
        private static readonly GedcomPointer AliceId = "@I93585620@".AsGedcomPointer()
        [SetUp]
        public void SetUp()
        {
            
        }

        public void GetParentsTest()
        {
            var db = GetDatabase("Relationships.ged");
            
            var alice = db.IndividualRecords[AliceId];

            alice.GetParents();
        }

        private GedcomDatabase GetDatabase(string resourceName)
        {
            using TextReader textReader = ResourceFactory.GetReader(GetType(), resourceName);
            var lineReader = new GedcomLineReader(textReader);
            var recordReader = new GedcomRecordReader(lineReader);
            GedcomDatabase result = new GedcomDatabase();
            result.Populate(recordReader);
            return result;
        }
    }
}