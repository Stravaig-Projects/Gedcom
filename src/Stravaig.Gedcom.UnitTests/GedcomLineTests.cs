using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomLineTests
    {
        [Test]
        public void ToString_MinimalLine_ReturnsValidLine()
        {
            GedcomLine line = new GedcomLine(0.AsGedcomLevel(), "HEAD".AsGedcomTag());
            line.ToString().ShouldBe("0 HEAD");
        }

        [Test]
        public void ToString_LineWithValue_ReturnsValidLine()
        {
            GedcomLine line = new GedcomLine(1.AsGedcomLevel(), "SOUR".AsGedcomTag(), "Stravaig.Gedcom");
            line.ToString().ShouldBe("1 SOUR Stravaig.Gedcom");
        }

        [Test]
        public void ToString_LineWithCrossReference_ReturnsValidLine()
        {
            GedcomLine line = new GedcomLine(0.AsGedcomLevel(), "@SUBM1@".AsGedcomPointer(), "SUBM".AsGedcomTag());
            line.ToString().ShouldBe("0 @SUBM1@ SUBM");
        }
        
        [Test]
        public void ToString_LineWithEverything_ReturnsValidLine()
        {
            GedcomLine line = new GedcomLine(0.AsGedcomLevel(), "@SOMEPOINTER@".AsGedcomPointer(), "_TEST".AsGedcomTag(), "This is a test value.");
            line.ToString().ShouldBe("0 @SOMEPOINTER@ _TEST This is a test value.");
        }
    }
}