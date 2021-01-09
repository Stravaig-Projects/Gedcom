using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.FamilyTree.Common.Humaniser;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // Instantiated by Paramore Brighter
    public class RenderPersonDescendantsAsJsonHandler : RenderPersonAsJsonHandlerBase
    {
        private class PersonModel
        {
            // Used by JSON Serialiser
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string Id { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Relationship { get; set; }
            public string DateOfBirth { get; set; }
            public string DateOfDeath { get; set; }
            public PersonModel[] Children { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
        
        private readonly IRelationshipRenderer _relationshipRenderer;
        private readonly IDateRenderer _dateRenderer;

        public RenderPersonDescendantsAsJsonHandler(ILogger<RenderPersonAncestorsAsJsonHandler> logger,
            IRelationshipRenderer relationshipRenderer,
            IFileNamer fileNamer,
            IDateRenderer dateRenderer)
            : base(logger, fileNamer)
        {
            _relationshipRenderer = relationshipRenderer;
            _dateRenderer = dateRenderer;
        }

        protected override string FileType => "descendants";

        protected override object MapIndividual(GedcomIndividualRecord subject)
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
                Id = subject.CrossReferenceId.ToSimpleIndividualId(),
                Gender = subject.Sex.ToString(),
                DateOfBirth = _dateRenderer.RenderAsShortDate(subject.BirthEvent?.Date),
                DateOfDeath = _dateRenderer.RenderAsShortDate(subject.DeathEvent?.Date),
                Children = subject.Children()
                    .OrderBy(r=>r.Relative.BirthEvent?.Date)
                    .ThenBy(r => r.Relative.Name)
                    .Select(MapRelative)
                    .ToArray(),
            };
        }

        private PersonModel MapRelative(ImmediateRelative relative)
        {
            var result = CreatePersonModel(relative.Relative);
            result.Relationship = _relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
            return result;
        }
    }
}