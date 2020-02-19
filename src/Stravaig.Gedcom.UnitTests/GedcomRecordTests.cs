using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomRecordTests
    {
        private static readonly GedcomLevel LevelZero = 0.AsGedcomLevel();
        private static readonly GedcomLevel LevelOne = 1.AsGedcomLevel();
        private static readonly GedcomTag HeadTag = "HEAD".AsGedcomTag();
        private static readonly GedcomTag SourceTag = "SOUR".AsGedcomTag();

        [Test]
        public void ctor_LineOnly_CreatesRecordWithoutParent()
        {
            GedcomLine line = new GedcomLine(LevelZero, HeadTag);
            GedcomRecord rec = new GedcomRecord(line);
            
            rec.Parent.ShouldBeNull();
            rec.Level.ShouldBe(LevelZero);
            rec.Tag.ShouldBe(HeadTag);
            rec.CrossReferenceId.ShouldBeNull();
            rec.Value.ShouldBeNull();
        }

        [Test]
        public void ctor_LineAndParent_CreatesRecordWithParentAndParentHasChild()
        {
            GedcomLine headLine = new GedcomLine(LevelZero, HeadTag);
            GedcomRecord headRecord = new GedcomRecord(headLine);

            GedcomLine sourceLine = new GedcomLine(LevelOne, SourceTag, "Some source");
            GedcomRecord sourceRecord = new GedcomRecord(sourceLine, headRecord);

            sourceRecord.Parent.ShouldBe(headRecord);
            headRecord.Children.Count.ShouldBe(1);
            headRecord.Children[0].ShouldBe(sourceRecord);
            sourceRecord.Value.ShouldBe("Some source");
        }

        [Test]
        public void Siblings()
        {
            GedcomLine headLine = new GedcomLine(LevelZero, HeadTag);
            GedcomRecord headRecord = new GedcomRecord(headLine);

            GedcomRecord sourceRecord = GedcomRecord.From("1 SOUR Some Source".AsGedcomLine(), headRecord);
            GedcomRecord.From("1 CHAR UTF-8".AsGedcomLine(), headRecord);
            GedcomRecord.From("1 GEDC".AsGedcomLine(), headRecord);
            GedcomRecord.From("1 PLAC".AsGedcomLine(), headRecord);
            GedcomRecord.From("1 SUBM @SUBM1@".AsGedcomLine(), headRecord);
            
            headRecord.Children.Count.ShouldBe(5);
            sourceRecord.SiblingsExclusive.Count.ShouldBe(4);
            sourceRecord.SiblingsInclusive.Count.ShouldBe(5);
            sourceRecord.SiblingsInclusive.ShouldContain(sourceRecord);
            sourceRecord.SiblingsExclusive.ShouldNotContain(sourceRecord);
        }
    }
}