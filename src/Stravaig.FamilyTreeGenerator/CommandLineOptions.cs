using CommandLine;

namespace Stravaig.FamilyTreeGenerator
{
    public class CommandLineOptions
    {
        [Option('s', "SourceFile", Required = true, HelpText = "The source GEDCOM file.")]
        public string SourceFile { get; set; }
        
        [Option('d', "DestinationFolder", Required = true, HelpText = "The destination folder.")]
        public string DestinationFolder { get; set; }
    }
}