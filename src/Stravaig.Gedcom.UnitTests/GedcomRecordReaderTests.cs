using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.UnitTests._helpers;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomRecordReaderTests : AsyncTestBase
    {

        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task ReadRecord_WithSingleLineRecord_ReturnsRecord(Read type)
        {
            using var textReader = new StringReader("0 HEAD");
            using var lineReader = new GedcomLineReader(textReader);
            var recordReader = new GedcomRecordReader(lineReader);

            var record = await ReadMaybeAsync(recordReader, type);
            
            record.Level.ShouldBe(0.AsGedcomLevel());
            record.Tag.ShouldBe("HEAD".AsGedcomTag());
            record.CrossReferenceId.ShouldBeNull();
            record.Value.ShouldBeNull();
            record.Parent.ShouldBeNull();
            record.SiblingsInclusive.Count.ShouldBe(0);
            record.Children.Count.ShouldBe(0);
                        
            (await ReadMaybeAsync(recordReader, type)).ShouldBeNull();
        }
        
        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task ReadRecord_WithSubordinateRecord_ReturnsRecordWithSingleChild(Read type)
        {
            using var textReader = new StringReader("0 HEAD\n1 SOUR SyniumFamilyTree");
            using var lineReader = new GedcomLineReader(textReader);
            var recordReader = new GedcomRecordReader(lineReader);

            var record = await ReadMaybeAsync(recordReader, type);
            
            record.Level.ShouldBe(0.AsGedcomLevel());
            record.Tag.ShouldBe("HEAD".AsGedcomTag());
            record.CrossReferenceId.ShouldBeNull();
            record.Value.ShouldBeNull();
            record.Parent.ShouldBeNull();
            record.SiblingsInclusive.Count.ShouldBe(0);
            record.Children.Count.ShouldBe(1);

            var child = record.Children.Single();
            child.Level.ShouldBe(1.AsGedcomLevel());
            child.Parent.ShouldBe(record);
            child.SiblingsExclusive.Count.ShouldBe(0);
            child.SiblingsInclusive.Count.ShouldBe(1);
            child.Children.Count.ShouldBe(0);
            child.Value.ShouldBe("SyniumFamilyTree");
            child.Tag.ShouldBe("SOUR".AsGedcomTag());
            
            (await ReadMaybeAsync(recordReader, type)).ShouldBeNull();
        }

        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task ReadRecord_WithCompleteExampleOfHeaderRecord_ReturnsRecord(Read type)
        {
            using TextReader textReader = ResourceFactory.GetReader(GetType(), "header.ged");
            using var lineReader = new GedcomLineReader(textReader);
            var recordReader = new GedcomRecordReader(lineReader);

            var record = await ReadMaybeAsync(recordReader, type);
            record.ShouldNotBeNull();
            record.Tag.ShouldBe("HEAD".AsGedcomTag());
            record.Children.Count.ShouldBe(5);
            record.Children[0].Tag.ShouldBe("SOUR".AsGedcomTag());
            record.Children[0].Children[0].Tag.ShouldBe("NAME".AsGedcomTag());
            record.Children[0].Children[1].Tag.ShouldBe("VERS".AsGedcomTag());
            record.Children[0].Children[2].Tag.ShouldBe("CORP".AsGedcomTag());
            record.Children[1].Tag.ShouldBe("CHAR".AsGedcomTag());
            record.Children[2].Tag.ShouldBe("GEDC".AsGedcomTag());
            record.Children[3].Tag.ShouldBe("PLAC".AsGedcomTag());
            record.Children[4].Tag.ShouldBe("SUBM".AsGedcomTag());

            var nextRecord = await ReadMaybeAsync(recordReader, type);
            nextRecord.ShouldBeNull();
        }
        
        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task ReadRecord_WithIncorrectLevel_ThrowsException(Read type)
        {
            using TextReader textReader = ResourceFactory.GetReader(GetType(), "header-jumps-levels.ged");
            using var lineReader = new GedcomLineReader(textReader);
            var recordReader = new GedcomRecordReader(lineReader);

            try
            {
                await ReadMaybeAsync(recordReader, type);
                Assert.Fail("Should not have got to this line.");
            }
            catch (GedcomReaderException ex)
            {
                Console.WriteLine(ex);
                ex.LineNumber.ShouldBe(4);
            }
        }
        
        private async Task<GedcomRecord> ReadMaybeAsync(GedcomRecordReader reader, Read type)
        {
            if (type == Read.Synchronous)
                return reader.ReadRecord();
            return await reader.ReadRecordAsync();
        }
    }
}