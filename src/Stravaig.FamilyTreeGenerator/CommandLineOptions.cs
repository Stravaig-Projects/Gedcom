using System.IO;
using CommandLine;

namespace Stravaig.FamilyTreeGenerator
{
    public class CommandLineOptions
    {
        [Option('f', "SourceFolder", Required = true, HelpText = "The folder containing the source files to be processed.")]
        public string SourceFolder { get; set; }

        [Option('m', "MediaFolder", Required = true, HelpText = "The folder containing the media files to be processed, relative to the Source Folder.")]
        public string MediaFolder { get; set; }

        [Option('s', "SourceFile", Required = true, HelpText = "The source GEDCOM file, relative to the Source Folder.")]
        public string SourceFile { get; set; }

        [Option('d', "DestinationFolder", Required = true, HelpText = "The destination folder.")]
        public string DestinationFolder { get; set; }

        public string FullMediaFilePath(string fileName) => Path.Combine(SourceFolder, MediaFolder, fileName);

        public string FullSourceFile() => Path.Combine(SourceFolder, SourceFile);
    }
}
