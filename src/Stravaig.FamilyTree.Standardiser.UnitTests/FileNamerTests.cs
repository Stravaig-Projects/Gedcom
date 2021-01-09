using System;
using System.Globalization;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.FamilyTree.Standardiser.UnitTests
{
    public class FileNamerTests
    {
        private const string TimestampFormat = @"yyyy-MM-dd\'HH-mm-ss";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleFileName_ReturnsWithDatePortionAdded()
        {
            string sourceFileName = @"c:/my/folder/path/my-file-name.ged";
            
            var timestamp = "2020-08-16'12-22-43";
            var result = new FileNamer(TimeFunc(timestamp))
                .RenameFile(sourceFileName);
            result.ShouldBe(@"c:\my\folder\path\"+ timestamp + ".old.my-file-name.ged");
        }

        [Test]
        public void FileNameThatsAlreadyBeenTimestamped_ReturnsWithDatePortionAdded()
        {
            string sourceFileName = @"c:/my/folder/path/2019-07-15'11-21-42.old.my-file-name.ged";
            
            var timestamp = "2020-08-16'12-22-43";
            var result = new FileNamer(TimeFunc(timestamp))
                .RenameFile(sourceFileName);
            result.ShouldBe(@"c:\my\folder\path\"+ timestamp + ".old.my-file-name.ged");
        }
        
        private static Func<DateTimeOffset> TimeFunc(string timestamp)
        {
            DateTimeOffset Func() => DateTimeOffset.ParseExact(
                timestamp,
                TimestampFormat,
                CultureInfo.InvariantCulture);
            return Func;
        }
    }
}