using System.Collections.Generic;
using System.IO;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Services
{
    public interface IFileNamer
    {
        FileInfo GetIndividualFile(GedcomIndividualRecord individual);
        IEnumerable<DirectoryInfo>  RequiredDirectories();
        DirectoryInfo BaseDirectory();
    }

    public class FileNamer : IFileNamer
    {
        private readonly CommandLineOptions _options;

        public FileNamer(CommandLineOptions options)
        {
            _options = options;
        }
        
        public FileInfo GetIndividualFile(GedcomIndividualRecord individual)
        {
            var peopleDir = PeopleDirectory();
            var fileName = $"{individual.CrossReferenceId}.md";
            var path = Path.Join(peopleDir.FullName, fileName);
            var absolutePath = Path.GetFullPath(path);
            return new FileInfo(absolutePath);
        }

        public IEnumerable<DirectoryInfo> RequiredDirectories()
        {
            yield return BaseDirectory();
            yield return PeopleDirectory();
        }

        public DirectoryInfo BaseDirectory()
        {
            var absoluteRootPath = Path.GetFullPath(_options.DestinationFolder);
            return new DirectoryInfo(absoluteRootPath);
        }

        private DirectoryInfo PeopleDirectory()
        {
            var peopleDirectory = Path.Join(BaseDirectory().FullName, "people");
            return new DirectoryInfo(peopleDirectory);
        }
    }
}