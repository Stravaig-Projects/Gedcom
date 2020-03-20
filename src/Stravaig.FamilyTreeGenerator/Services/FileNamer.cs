using System.Collections.Generic;
using System.IO;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Services
{
    public interface IFileNamer
    {
        string GetIndividualFile(GedcomIndividualRecord individual, string relativeTo = null);
        string GetByNameIndexFile(string relativeTo = null);
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
        
        public string GetIndividualFile(GedcomIndividualRecord individual, string relativeTo = null)
        {
            var peopleDir = PeopleDirectory(relativeTo);
            var personName = individual.NameWithoutMarker
                .MakeFileNameSafe()
                ?.Replace(" ", "-") ?? "X";
            var birth = individual.BirthEvent?.Date?.RawDateValue
                .MakeFileNameSafe()
                ?.Replace(" ", "-") ?? "X";
            var death = individual.DeathEvent?.Date?.RawDateValue
                .MakeFileNameSafe()
                ?.Replace(" ", "-") ?? "X";
            var fileName = $"{individual.CrossReferenceId}-{personName}-b{birth}-d{death}.md";
            var path = Path.Join(peopleDir, fileName);
            path = path.Replace("\\", "/");
            
            return path;
        }

        public string GetByNameIndexFile(string relativeTo = null)
        {
            const string fileName = "Index-ByName.md";
            var baseDirectory = BaseDirectory();
            var path = Path.Join(baseDirectory.FullName, "Index-ByName.md");
            if (relativeTo != null)
                path = Path.GetRelativePath(relativeTo, path);
            path = path.Replace("\\", "/");
            return path;
        }

        public IEnumerable<DirectoryInfo> RequiredDirectories()
        {
            yield return BaseDirectory();
            yield return new DirectoryInfo(PeopleDirectory());
        }

        public DirectoryInfo BaseDirectory()
        {
            var absoluteRootPath = Path.GetFullPath(_options.DestinationFolder);
            return new DirectoryInfo(absoluteRootPath);
        }

        private string PeopleDirectory(string relativeTo = null)
        {
            var peopleDirectory = Path.Join(BaseDirectory().FullName, "people");
            if (relativeTo != null)
                peopleDirectory = Path.GetRelativePath(relativeTo, peopleDirectory);
            return peopleDirectory;
        }
    }
}