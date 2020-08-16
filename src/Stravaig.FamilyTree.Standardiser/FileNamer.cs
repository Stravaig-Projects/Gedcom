using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Stravaig.FamilyTree.Standardiser
{
    public class FileNamer
    {
        private const string TimestampFormat = @"yyyy-MM-dd\'HH-mm-ss";
        private static readonly Regex PreviousTimestampRegex = new Regex(@"^\d{4}(-\d\d){2}\'(\d\d-){2}\d\d\.old\.");
        private readonly Func<DateTimeOffset> _now;

        public FileNamer(Func<DateTimeOffset> now)
        {
            _now = now;
        }
        public string RenameFile(string sourceFile)
        {
            FileInfo fi = new FileInfo(sourceFile);
            string directoryPart = fi.Directory.FullName;
            string fileNamePart = fi.Name;
            var match = PreviousTimestampRegex.Match(fileNamePart);
            if (match.Success)
            {
                fileNamePart = fileNamePart.Substring(match.Length);
            }

            var timestamp = _now().ToString(TimestampFormat);
            string result = $"{directoryPart}{Path.DirectorySeparatorChar}{timestamp}.old.{fileNamePart}";
            result = Path.GetFullPath(result);
            return result;
        }
    }
}