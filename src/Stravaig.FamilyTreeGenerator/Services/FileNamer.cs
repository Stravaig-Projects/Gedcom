using System.Collections.Generic;
using System.IO;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Services
{
    public interface IFileNamer
    {
        string GetIndividualFile(GedcomIndividualRecord individual, string relativeTo = null);
        string GetIndividualFile(GedcomIndividualRecord individual, GedcomIndividualRecord relativeTo);

        string GetDataFile(GedcomIndividualRecord individual, string filename);

        
        string GetSourceFile(GedcomSourceRecord source, string relativeTo = null);
        string GetSourceFile(GedcomSourceRecord source, GedcomIndividualRecord relativeTo);
        
        string GetByNameIndexFile(string relativeTo = null);
        string GetByAllNamesIndexFile(string relativeTo = null);

        string GetByDateOfBirthIndexFile(string relativeTo = null);
        string GetByUnknownDateOfBirthIndexFile(string relativeTo = null);
        string GetByBirthLocationIndexFile(string relativeTo = null);
        string GetByResidenceLocationIndexFile(string relativeTo = null);
        string GetByDeathLocationIndexFile(string relativeTo = null);
        string GetByOccupationIndexFile(string relativeTo = null);

        string GetByMarriageByDateIndexFile(string relativeTo = null);
        string GetByMarriageByNameIndexFile(string relativeTo = null);

        string GetSourceIndexFile(string relativeTo = null);
        
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
        
        public string GetIndividualFile(GedcomIndividualRecord individual, GedcomIndividualRecord relativeTo)
        {
            var individualFile = GetIndividualFile(relativeTo);
            var thisDirectory = new FileInfo(individualFile).DirectoryName;
            return GetIndividualFile(individual, thisDirectory);
        }
        public string GetIndividualFile(GedcomIndividualRecord individual, string relativeTo = null)
        {
            var peopleDir = PeopleDirectory(relativeTo);
            var personName = individual.NameWithoutMarker
                .MakeFileNameSafe()
                ?? "X";
            var birth = individual.BirthEvent?.Date.ForFileName();
            var death = individual.DeathEvent?.Date.ForFileName();
            var fileName = $"{individual.CrossReferenceId}-{personName}-b{birth}-d{death}.md".ToLowerInvariant();
            var path = Path.Join(peopleDir, fileName);
            path = path.Replace("\\", "/");
            
            return path;
        }

        public string GetDataFile(GedcomIndividualRecord individual, string filename)
        {
            string id = individual.CrossReferenceId.ToSimpleIndividualId();
            string dir = GetDataDirectory(id);
            var path = Path.Join(dir, filename);
            return path;
        }
        
        public string GetSourceFile(GedcomSourceRecord source, string relativeTo = null)
        {
            var sourceDir = SourceDirectory(relativeTo);
            var title = source.Title.MakeFileNameSafe();
            var fileName = $"{source.CrossReferenceId}-{title}.md"
                .ToLowerInvariant();

            var path = Path.Join(sourceDir, fileName);
            path = path.Replace("\\", "/");
            return path;
        }

        public string GetSourceFile(GedcomSourceRecord source, GedcomIndividualRecord relativeTo)
        {
            var thisDirectory = new FileInfo(GetIndividualFile(relativeTo)).DirectoryName;
            return GetSourceFile(source, thisDirectory);
        }


        public string GetByNameIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-family-name.md";
            return GetIndexFile(fileName, relativeTo);
        }
        
        public string GetByAllNamesIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-all-names.md";
            return GetIndexFile(fileName, relativeTo);
        }
        
        public string GetByBirthLocationIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-birth-location.md";
            return GetIndexFile(fileName, relativeTo);
        }

        public string GetByResidenceLocationIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-residence-location.md";
            return GetIndexFile(fileName, relativeTo);
        }

        public string GetByDeathLocationIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-death-location.md";
            return GetIndexFile(fileName, relativeTo);
        }
        
        public string GetByDateOfBirthIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-date-of-birth.md";
            return GetIndexFile(fileName, relativeTo);
        }
        
        public string GetByUnknownDateOfBirthIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-unknown-date-of-birth.md";
            return GetIndexFile(fileName, relativeTo);
        }

        public string GetSourceIndexFile(string relativeTo = null)
        {
            const string fileName = "index-of-sources-by-title.md";
            return GetIndexFile(fileName, relativeTo);
        }

        public string GetByMarriageByDateIndexFile(string relativeTo = null)
        {
            const string fileName = "index-marriage-by-date.md";
            return GetIndexFile(fileName, relativeTo);
        }
        
        public string GetByMarriageByNameIndexFile(string relativeTo = null)
        {
            const string fileName = "index-marriage-by-name.md";
            return GetIndexFile(fileName, relativeTo);
        }

        public string GetByOccupationIndexFile(string relativeTo = null)
        {
            return GetIndexFile("index-by-occupation.md", relativeTo);
        }

        public IEnumerable<DirectoryInfo> RequiredDirectories()
        {
            yield return BaseDirectory();
            yield return new DirectoryInfo(PeopleDirectory());
            yield return new DirectoryInfo(SourceDirectory());
        }

        public DirectoryInfo BaseDirectory()
        {
            var absoluteRootPath = Path.GetFullPath(_options.DestinationFolder);
            return new DirectoryInfo(absoluteRootPath);
        }

        private string GetIndexFile(string fileName, string relativeTo)
        {
            var baseDirectory = BaseDirectory();
            var path = Path.Join(baseDirectory.FullName, fileName);
            if (relativeTo != null)
                path = Path.GetRelativePath(relativeTo, path);
            path = path.Replace("\\", "/");
            return path;
        }
        
        private string PeopleDirectory(string relativeTo = null)
        {
            var peopleDirectory = Path.Join(BaseDirectory().FullName, "people");
            if (relativeTo != null)
                peopleDirectory = Path.GetRelativePath(relativeTo, peopleDirectory);
            return peopleDirectory;
        }
        private string SourceDirectory(string relativeTo = null)
        {
            var sourceDirectory = Path.Join(BaseDirectory().FullName, "sources");
            if (relativeTo != null)
                sourceDirectory = Path.GetRelativePath(relativeTo, sourceDirectory);
            return sourceDirectory;
        }

        private string GetDataDirectory(string id)
        {
            var dataDirectory = Path.Join(BaseDirectory().FullName, $"data/{id}");
            return dataDirectory;
        }
    }
}