using System;
using System.Collections.Generic;
using System.IO;
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
        string GetByDateOfBirthIndexFile(string relativeTo = null);

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
            string id = individual.CrossReferenceId.ToString().Trim('@').ToLowerInvariant();
            string dir = GetDataDirectory(id);
            var path = Path.Join(dir, filename);
            return path;
        }
        
        public string GetSourceFile(GedcomSourceRecord source, string relativeTo = null)
        {
            var sourceDir = SourceDirectory(relativeTo);
            var title = source.Title.MakeFileNameSafe();
            var fileName = $"{source.CrossReferenceId}-{title}.md".ToLowerInvariant();
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
            var baseDirectory = BaseDirectory();
            var path = Path.Join(baseDirectory.FullName, fileName);
            if (relativeTo != null)
                path = Path.GetRelativePath(relativeTo, path);
            path = path.Replace("\\", "/");
            return path;
        }

        public string GetByDateOfBirthIndexFile(string relativeTo = null)
        {
            const string fileName = "index-by-date-of-birth.md";
            var baseDirectory = BaseDirectory();
            var path = Path.Join(baseDirectory.FullName, fileName);
            if (relativeTo != null)
                path = Path.GetRelativePath(relativeTo, path);
            path = path.Replace("\\", "/");
            return path;
        }

        public string GetSourceIndexFile(string relativeTo = null)
        {
            const string fileName = "index-of-sources-by-title.md";
            var baseDirectory = BaseDirectory();
            var path = Path.Join(baseDirectory.FullName, fileName);
            if (relativeTo != null)
                path = Path.GetRelativePath(relativeTo, path);
            path = path.Replace("\\", "/");
            return path;
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