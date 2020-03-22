using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.UnitTests._helpers;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomIndividualRecordTests
    {
        [Test]
        public void Test()
        {
            var individualRecord = GetIndividualRecord("IndividualOne.ged");

            individualRecord.Name.ShouldBe("Iain /Menȝies/");
            individualRecord.Sex.ShouldBe(GedcomSex.Male);
            individualRecord.CrossReferenceId.ShouldBe("@I123@".AsGedcomPointer());
        }

        private GedcomIndividualRecord GetIndividualRecord(string resourceName)
        {
            using var reader = ResourceFactory.GetReader(GetType(), resourceName);
            using var lr = new GedcomLineReader(reader);
            GedcomRecordReader rr = new GedcomRecordReader(lr);
            var record = rr.ReadRecord();
            var individualRecord = new GedcomIndividualRecord(record);
            return individualRecord;
        }
    }
}