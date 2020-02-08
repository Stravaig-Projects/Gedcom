using System;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomLineParserTests
    {
        private GedcomLineParser _parser;
        
        [SetUp]
        public void SetUp()
        {
            _parser = new GedcomLineParser();
        }
    
        [Test]
        public void Parse_LineIsNull_ThrowsException()
        {
            Should.Throw<ArgumentException>(() => _parser.Parse(null));
        }

        [Test]
        public void Parse_LineIsEmpty_ThrowsException()
        {
            Should.Throw<ArgumentException>(() => _parser.Parse(string.Empty));
        }

        [Test]
        public void Parse_LineIsWhiteSpace_ThrowsException()
        {
            Should.Throw<ArgumentException>(() => _parser.Parse(" ")); 
        }

        [Test]
        public void Parse_LineIsTooLong_ThrowsException()
        {
            var ex = Should.Throw<GedcomLineParserException>(
                () => _parser.Parse(new string('*', 256)));
            ex.CharacterPosition.ShouldBe(0);
            ex.Message.ShouldBe("Line cannot be longer than 255 characters.");
        }

        [Test]
        public void Parse_MinimalLine_ResultsInALineObjectBeingReturned()
        {
            const string lineString = "0 TAG";
            GedcomLine line = _parser.Parse(lineString);
            line.Level.ShouldBe(new GedcomLevel(0));
            line.Tag.ShouldBe(new GedcomTag("TAG"));
            line.CrossReferenceId.ShouldBeNull();
            line.Value.ShouldBeNull();
        }
        
        [Test]
        public void Parse_FullLine_ResultsInALineObjectBeingReturned()
        {
            var level = new GedcomLevel(1);
            var pointer = new GedcomPointer("@POINTER@");
            var tag = new GedcomTag("OTHER_TAG");
            var value = "This is the value that makes up the remainder.";
            string lineString = $"{level} {pointer} {tag} {value}";
            
            GedcomLine line = _parser.Parse(lineString);
            line.Level.ShouldBe(level);
            line.Tag.ShouldBe(tag);
            line.CrossReferenceId.ShouldBe(pointer);
            line.Value.ShouldBe(value);
        }
        
        [Test]
        public void Parse_LineWithInvalidLevel_ThrowsException()
        {
            string lineString = $"00 SOME_TAG";
            var ex = Should.Throw<GedcomLineParserException>(
                () => _parser.Parse(lineString));
            ex.CharacterPosition.ShouldBe(0);
            ex.Message.ShouldBe("Expected a valid level value, at character position 0.");
            ex.InnerException.ShouldNotBeNull();
            ex.InnerException.ShouldBeOfType<ArgumentException>();
        }

        [Test]
        public void Parse_LineWithInvalidCrossReferenceId_ThrowsException()
        {
            string lineString = $"0 @@ SOME_TAG";
            var ex = Should.Throw<GedcomLineParserException>(
                () => _parser.Parse(lineString));
            ex.CharacterPosition.ShouldBe(2);
            ex.Message.ShouldBe("Expected a CrossReferenceId or Tag value, at character position 2.");
        }
        
        [Test]
        public void Parse_LineWithInvalidTag_ThrowsException()
        {
            string lineString = $"0 T~A-G";
            var ex = Should.Throw<GedcomLineParserException>(
                () => _parser.Parse(lineString));
            ex.CharacterPosition.ShouldBe(2);
            ex.Message.ShouldBe("Expected a CrossReferenceId or Tag value, at character position 2.");
        }

        [Test]
        public void Parse_LineWithInvalidTagAfterCrossReference_ThrowsException()
        {
            string lineString = $"0 @POINTER@ T~A-G";
            var ex = Should.Throw<GedcomLineParserException>(
                () => _parser.Parse(lineString));
            ex.CharacterPosition.ShouldBe(12);
            ex.Message.ShouldBe("Expected a tag value, at character position 12.");
        }
    }
}