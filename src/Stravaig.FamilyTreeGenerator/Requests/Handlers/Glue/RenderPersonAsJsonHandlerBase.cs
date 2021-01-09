using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public abstract class RenderPersonAsJsonHandlerBase : RequestHandler<RenderIndividual>
    {
        private readonly ILogger _logger;
        private readonly IFileNamer _fileNamer;

        protected RenderPersonAsJsonHandlerBase(ILogger logger, IFileNamer fileNamer)
        {
            _logger = logger;
            _fileNamer = fileNamer;
        }
        
        protected string GetFileName(RenderIndividual command)
        {
            var fileName = _fileNamer.GetDataFile(command.Individual, $"{FileType}.json");
            FileInfo fi = new FileInfo(fileName);
            DirectoryInfo di = fi.Directory;
            if (!di.Exists)
                di.Create();
            return fileName;
        }
        
        protected abstract string FileType { get; }
        
        public override RenderIndividual Handle(RenderIndividual command)
        {
            _logger.LogInformation(
                $"Render {FileType} As Json {command.Individual.CrossReferenceId} : {command.Individual.Name}");

            var result = MapIndividual(command.Individual);
            var fileName = GetFileName(command);

            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, result);
            
            return base.Handle(command);
        }

        protected static string GetName(GedcomIndividualRecord subject)
        {
            if (subject.IsAlive())
                return "X";
            return subject.NameWithoutMarker;
        }
        
        protected abstract object MapIndividual(GedcomIndividualRecord subject);
    }
}