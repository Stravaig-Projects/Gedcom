using System;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomLineBuilderTests
    {
        [Test]
        public void Build_OnUnusedBuilder_ThrowsException()
        {
            GedcomLineBuilder builder = new GedcomLineBuilder();
            Should.Throw<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void Build_OnFullLine_CreatesGedcomLineObject()
        {
            GedcomLine line = new GedcomLineBuilder()
                .SetLevel(0)
                .SetCrossReferenceId("@A@")
                .SetTag("TAG")
                .SetValue("VALUE")
                .Build();

            line.ShouldNotBeNull();
            line.Level.ShouldBe(new GedcomLevel(0));
            line.CrossReferenceId.ShouldBe(new GedcomPointer("@A@"));
            line.Tag.ShouldBe(new GedcomTag("TAG"));
            line.Value.ShouldBe("VALUE");
        }
        
        [Test]
        public void Build_WithoutCrossReference_CreatesGedcomLineObject()
        {
            GedcomLine line = new GedcomLineBuilder()
                .SetLevel(0)
                .SetTag("TAG")
                .SetValue("VALUE")
                .Build();

            line.ShouldNotBeNull();
            line.Level.ShouldBe(new GedcomLevel(0));
            line.CrossReferenceId.ShouldBe(null);
            line.Tag.ShouldBe(new GedcomTag("TAG"));
            line.Value.ShouldBe("VALUE");
        }
        
        [Test]
        public void Build_WithoutValue_CreatesGedcomLineObject()
        {
            GedcomLine line = new GedcomLineBuilder()
                .SetLevel(0)
                .SetCrossReferenceId("@A@")
                .SetTag("TAG")
                .Build();

            line.ShouldNotBeNull();
            line.Level.ShouldBe(new GedcomLevel(0));
            line.CrossReferenceId.ShouldBe(new GedcomPointer("@A@"));
            line.Tag.ShouldBe(new GedcomTag("TAG"));
            line.Value.ShouldBe(null);
        }

        [Test]
        public void Build_WithMinimalItems_CreatesGedcomLineObject()
        {
            GedcomLine line = new GedcomLineBuilder()
                .SetLevel(0)
                .SetTag("TAG")
                .Build();

            line.ShouldNotBeNull();
            line.Level.ShouldBe(new GedcomLevel(0));
            line.CrossReferenceId.ShouldBe(null);
            line.Tag.ShouldBe(new GedcomTag("TAG"));
            line.Value.ShouldBe(null);
        }
        
        [Test]
        public void Build_WithMissingLevel_ThrowsException()
        {
            GedcomLineBuilder builder = new GedcomLineBuilder()
                .SetTag("TAG");

            Should.Throw<InvalidOperationException>(()=> builder.Build())
                .Message.ShouldBe("Cannot build a valid GedcomLine because a level is required.");
        }
        
        [Test]
        public void Build_WithMissingTag_ThrowsException()
        {
            GedcomLineBuilder builder = new GedcomLineBuilder()
                .SetLevel(0);

            Should.Throw<InvalidOperationException>(()=> builder.Build())
                .Message.ShouldBe("Cannot build a valid GedcomLine because a tag is required.");
        }
        
        [Test]
        public void BuildResetBuild_HappyPath_CreatesGedcomLineObjectWithSecondSetOfProperties()
        {
            var builder = new GedcomLineBuilder();
            
            GedcomLine line = builder
                .SetLevel(0)
                .SetCrossReferenceId("@A@")
                .SetTag("TAG")
                .SetValue("VALUE")
                .Build();

            builder.Reset();

            line = builder
                .SetLevel(1)
                .SetTag("NEWTAG")
                .Build();
            
            line.ShouldNotBeNull();
            line.Level.ShouldBe(new GedcomLevel(1));
            line.CrossReferenceId.ShouldBeNull();
            line.Tag.ShouldBe(new GedcomTag("NEWTAG"));
            line.Value.ShouldBeNull();
        }
    }
}