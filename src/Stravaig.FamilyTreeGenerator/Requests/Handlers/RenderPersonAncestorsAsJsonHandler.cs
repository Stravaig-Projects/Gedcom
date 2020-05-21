using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonAncestorsAsJsonHandler : RequestHandler<RenderIndividual>
    {
        private class PersonModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Relationship { get; set; }
            public string DateOfBirth { get; set; }
            public string DateOfDeath { get; set; }
            public PersonModel[] Parents { get; set; }
        }
        
        private readonly ILogger<RenderPersonAncestorsAsJsonHandler> _logger;
        private readonly IRelationshipRenderer _relationshipRenderer;
        private readonly IFileNamer _fileNamer;
        private readonly IDateRenderer _dateRenderer;

        public RenderPersonAncestorsAsJsonHandler(ILogger<RenderPersonAncestorsAsJsonHandler> logger,
            IRelationshipRenderer relationshipRenderer,
            IFileNamer fileNamer,
            IDateRenderer dateRenderer)
        {
            _logger = logger;
            _relationshipRenderer = relationshipRenderer;
            _fileNamer = fileNamer;
            _dateRenderer = dateRenderer;
        }

        public override RenderIndividual Handle(RenderIndividual command)
        {
            _logger.LogInformation(
                $"Render Ancestors As Json {command.Individual.CrossReferenceId} : {command.Individual.Name}");

            var result = MapIndividual(command.Individual);
            var fileName = GetFileName(command);

            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, result);
            
            return base.Handle(command);
        }

        private string GetFileName(RenderIndividual command)
        {
            var fileName = _fileNamer.GetDataFile(command.Individual, "ancestors.json");
            FileInfo fi = new FileInfo(fileName);
            DirectoryInfo di = fi.Directory;
            if (!di.Exists)
                di.Create();
            return fileName;
        }

        private PersonModel MapIndividual(GedcomIndividualRecord subject)
        {
            var result = CreatePersonModel(subject);
            result.Relationship = "Self";
            return result;
        }

        private PersonModel CreatePersonModel(GedcomIndividualRecord subject)
        {
            return new PersonModel
            {
                Name = GetName(subject),
                Id = subject.CrossReferenceId.ToSimpleId(),
                Gender = subject.Sex.ToString(),
                DateOfBirth = _dateRenderer.RenderAsShortDate(subject.BirthEvent?.Date),
                DateOfDeath = _dateRenderer.RenderAsShortDate(subject.DeathEvent?.Date),
                Parents = subject.Parents().Select(MapRelative).ToArray(),
            };
        }

        private static string GetName(GedcomIndividualRecord subject)
        {
            if (subject.IsAlive())
                return "X";
            return subject.NameWithoutMarker;
        }

        private PersonModel MapRelative(ImmediateRelative relative)
        {
            var result = CreatePersonModel(relative.Relative);
            result.Relationship = _relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
            return result;
        }
    }
}