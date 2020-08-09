using CommandLine;

namespace Stravaig.FamilyTree.Standardiser
{
    public class CommandLineOptions
    {
        [Option('s', "SourceFile", Required = true, HelpText = "The source GEDCOM file.")]
        public string SourceFile { get; set; }
        
        [Option('d', "DestinationFile", Required = true, HelpText = "The destination GEDCOM file. If the destination is the same as the source, the source will be renamed with a 'old' prefix.")]
        public string DestinationFolder { get; set; }
    }
}