using System.IO;
using CommandLine;

namespace Stravaig.FamilyTree.Standardiser
{
    public class CommandLineOptions
    {
        private string _sourceFile;
        private string _destinationFile;

        [Option('s', "SourceFile", Required = true, HelpText = "The source GEDCOM file.")]
        public string SourceFile
        {
            get => _sourceFile;
            set
            {
                var absolutePath = Path.GetFullPath(value);
                if (File.Exists(absolutePath))
                    _sourceFile = absolutePath;
                else
                    throw new FileNotFoundException($"The file specified, \"{value}\" was not found.", absolutePath);
            }
        }

        [Option('d', "DestinationFile", Required = true,
            HelpText =
                "The destination GEDCOM file. If the destination is the same as the source, the source will be renamed with a 'old' prefix.")]
        public string DestinationFile
        {
            get => _destinationFile;
            set
            {
                var absolutePath = Path.GetFullPath(value);
                _destinationFile = absolutePath;
            }
        }
    }
}