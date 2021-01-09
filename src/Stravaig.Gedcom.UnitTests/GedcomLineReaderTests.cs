using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.UnitTests._helpers;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomLineReaderTests : AsyncTestBase
    {
        [Test]
        public void EnsureThatUnderlyingReaderIsClosedOnDispose()
        {
            using StringReader stringReader = new StringReader(string.Empty);
            using (new GedcomLineReader(stringReader))
            {
                // Do nothing here. Just going to dispose directly.
            }
            Should.Throw<ObjectDisposedException>(() => stringReader.ReadToEnd());
        }

        [Test]
        public void IsAtEnd_InitialState_ReturnsFalse()
        {
            using StringReader stringReader = new StringReader(string.Empty);
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);

            gedcomReader.IsAtEnd.ShouldBeFalse();
        }
        
        [Test]
        public void LineNumber_InitialState_ReturnsOne()
        {
            using StringReader stringReader = new StringReader(string.Empty);
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);

            gedcomReader.LineNumber.ShouldBe(1);
        }
        
        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task Read_EmptyFile_ReturnsNull(Read type)
        {
            using StringReader stringReader = new StringReader(string.Empty);
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);

            GedcomLine result = await ReadMaybeAsync(gedcomReader, type);
            
            result.ShouldBeNull();
            gedcomReader.IsAtEnd.ShouldBeTrue();
            gedcomReader.LineNumber.ShouldBe(1);
        }

        [Test]
        [TestCaseSource(nameof(ReadTypes))]
        public async Task Read_SingleLineFile_ReturnsOneLine(Read type)
        {
            using StringReader stringReader = new StringReader("0 HEAD");
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);

            GedcomLine result = await ReadMaybeAsync(gedcomReader, type);
            
            result.ShouldNotBeNull();
            result.Level.ShouldBe(0.AsGedcomLevel());
            result.Tag.ShouldBe("HEAD".AsGedcomTag());
            gedcomReader.IsAtEnd.ShouldBeFalse();
            gedcomReader.LineNumber.ShouldBe(2);
            
            result = await ReadMaybeAsync(gedcomReader, type);
            result.ShouldBeNull();
            gedcomReader.IsAtEnd.ShouldBeTrue();
            
            gedcomReader.LineNumber.ShouldBe(2);
        }

        [Test]
        [TestCaseSource(nameof(DummyFiles))]
        public async Task Read_MultipleGoodLines_ReturnsGedcomLineForeachLine(ValueTuple<Read, string> data)
        {
            Read type = data.Item1;
            string fileContent = data.Item2;
            using StringReader stringReader = new StringReader(fileContent);
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);

            List<GedcomLine> lines = new List<GedcomLine>();
            while (gedcomReader.IsAtEnd == false)
            {
                GedcomLine result = await ReadMaybeAsync(gedcomReader, type);
                lines.Add(result);
            }
            
            lines[0].Level.ShouldBe(0.AsGedcomLevel());
            lines[0].Tag.ShouldBe("HEAD".AsGedcomTag());
            
            lines[1].Level.ShouldBe(1.AsGedcomLevel());
            lines[1].Tag.ShouldBe("SOUR".AsGedcomTag());
            
            lines[2].Level.ShouldBe(2.AsGedcomLevel());
            lines[2].Tag.ShouldBe("NAME".AsGedcomTag());
            
            lines[3].Level.ShouldBe(2.AsGedcomLevel());
            lines[3].Tag.ShouldBe("VERS".AsGedcomTag());
            
            lines[4].ShouldBeNull();
            lines.Count.ShouldBe(5);
        }

        [Test]
        [TestCaseSource(nameof(DummyFiles))]
        public async Task Read_RunOffEndOfFile_StopsAdvancingLineNumber(ValueTuple<Read, string> data)
        {
            Read type = data.Item1;
            string fileContent = data.Item2;
            using StringReader stringReader = new StringReader(fileContent);
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);
            while (gedcomReader.IsAtEnd == false)
            {
                // Run to the end of the file.
                await ReadMaybeAsync(gedcomReader, type);
            }

            int lineNumber = gedcomReader.LineNumber;
            (await ReadMaybeAsync(gedcomReader, type)).ShouldBeNull();
            gedcomReader.IsAtEnd.ShouldBeTrue();
            gedcomReader.LineNumber.ShouldBe(lineNumber);
        }
        
        [Test]
        [TestCaseSource(nameof(DodgyDummyFiles))]
        public async Task Read_BadFile_ThrowsExceptions(ValueTuple<Read, string> data)
        {
            Read type = data.Item1;
            string fileContent = data.Item2;
            using StringReader stringReader = new StringReader(fileContent);
            using GedcomLineReader gedcomReader = new GedcomLineReader(stringReader);

            // Line 1
            try
            {
                await ReadMaybeAsync(gedcomReader, type);
            }
            catch (GedcomReaderException ex)
            {
                Console.WriteLine(ex);
                ex.LineNumber.ShouldBe(1);
                ex.CharacterPosition.ShouldBe(2);
                ex.Message.ShouldBe("An error occurred at line 1, character 2 of the GEDCOM file. See inner exception for more details.");
            }
        }

        private async Task<GedcomLine> ReadMaybeAsync(GedcomLineReader reader, Read type)
        {
            if (type == Read.Synchronous)
                return reader.ReadLine();
            return await reader.ReadLineAsync();
        }
        
        private static IEnumerable<(Read, string)> DummyFiles()
        {
            string[] files =
            {
                "0 HEAD\n1 SOUR StravaigGedcom\n2 NAME StravaigGedcom Nuget Package\n2 VERS 0.1",
                "0 HEAD\n\n1 SOUR StravaigGedcom\n   \n2 NAME StravaigGedcom Nuget Package\n        \n2 VERS 0.1\n\n\n",
            };
            foreach(Read type in ReadTypes())
            foreach (string file in files)
                yield return (type, file);
        }
        
        private static IEnumerable<(Read, string)> DodgyDummyFiles()
        {
            string[] files =
            {
                "0 HEAD_TAG_WITH_WAY_TO_MANY_CHARACTERS_IN_IT\n1 SOUR-NOT-A-TAG StravaigGedcom\n2 @POINTER@ NAME StravaigGedcom Nuget Package\n2 @***DODGY POINTER***@ VERS 0.1",
                "0 HEAD_TAG_WITH_WAY_TO_MANY_CHARACTERS_IN_IT\n\n1 SOUR-NOT-A-TAG StravaigGedcom\n   \n2 @POINTER@ NAME StravaigGedcom Nuget Package\n        \n2 @***DODGY POINTER***@ VERS 0.1\n\n\n",
            };
            foreach(Read type in ReadTypes())
            foreach (string file in files)
                yield return (type, file);
        }
    }
}