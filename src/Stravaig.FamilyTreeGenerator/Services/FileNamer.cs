using System.Collections.Generic;
using System.IO;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Services
{
    public interface IFileNamer
    {
        string GetIndividualFile(GedcomIndividualRecord individual, bool relativeToRoot = false, bool withHttpSlash = false);
        string GetByNameIndexFile(bool relativeToRoot = false, bool withHttpSlash = false);
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
        
        public string GetIndividualFile(GedcomIndividualRecord individual, bool relativeToRoot = false, bool withHttpSlash = false)
        {
            var peopleDir = PeopleDirectory(relativeToRoot);
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
            if (relativeToRoot == false) path = Path.GetFullPath(path);
            if (withHttpSlash) path = path.Replace("\\", "/");
            return path;
        }

        public string GetByNameIndexFile(bool relativeToRoot = false, bool withHttpSlash = false)
        {
            const string fileName = "Index-ByName.md";
            if (relativeToRoot)
                return fileName;
            var baseDirectory = BaseDirectory();
            var path = Path.Join(baseDirectory.FullName, "Index-ByName.md");
            if (withHttpSlash) path = path.Replace("\\", "/");
            return path;
        }

        public IEnumerable<DirectoryInfo> RequiredDirectories()
        {
            yield return BaseDirectory();
            yield return new DirectoryInfo(PeopleDirectory(false));
        }

        public DirectoryInfo BaseDirectory()
        {
            var absoluteRootPath = Path.GetFullPath(_options.DestinationFolder);
            return new DirectoryInfo(absoluteRootPath);
        }

        private string PeopleDirectory(bool relativeToRoot)
        {
            if (relativeToRoot)
                return "people";
            var peopleDirectory = Path.Join(BaseDirectory().FullName, "people");
            return peopleDirectory;
        }
    }
}