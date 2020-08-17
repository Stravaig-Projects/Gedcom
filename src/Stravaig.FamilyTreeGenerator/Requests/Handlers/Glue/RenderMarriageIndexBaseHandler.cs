using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public abstract class RenderMarriageIndexBaseHandler : RenderPersonIndexBaseHandler
    {
        protected IIndividualNameRenderer _nameRenderer;

        public RenderMarriageIndexBaseHandler(ILogger logger, 
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer) 
            : base(logger, fileNamer)
        {
            _nameRenderer = nameRenderer;
        }

        protected static readonly GedcomTag[] MarriageTags = new[]
        {
            GedcomFamilyEventRecord.MarriageTag,
        };

        protected sealed override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
        {
            var marriedFamilies = people.SelectMany(p => p.SpouseToFamilies)
                .Distinct()
                .Where(f => f.Events.Any(e => MarriageTags.Contains(e.Tag)))
                .ToArray();

            WriteIndex(writer, marriedFamilies);
        }

        protected abstract void WriteIndex(TextWriter writer, GedcomFamilyRecord[] families);
        
        protected string RenderPartner(GedcomIndividualRecord person)
        {
            if (person.IsAlive())
                return $"X";

            return _nameRenderer.RenderNameWithLifespan(person, linkName: true, boldName: true);
        }
    }
}