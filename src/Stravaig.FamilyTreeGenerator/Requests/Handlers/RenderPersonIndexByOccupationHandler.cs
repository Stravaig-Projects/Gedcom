using System.IO;
using System.Linq;
using Humanizer;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByOccupationHandler : RenderPersonIndexBaseHandler
    {
        private readonly IIndividualNameRenderer _nameRenderer;

        public class Entry
        {
            public GedcomIndividualRecord Subject;
            public GedcomIndividualAttributeRecord Occupation;
            public int Count;
            public GedcomDateRecord EarliestDate;
            public GedcomDateRecord LatestDate;
        }
        public RenderPersonIndexByOccupationHandler(ILogger<RenderPersonIndexByOccupationHandler> logger, 
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer) 
            : base(logger, fileNamer)
        {
            _nameRenderer = nameRenderer;
        }

        protected override string FileName => _fileNamer.GetByOccupationIndexFile();
        protected override void WriteIndex(TextWriter writer, 
            GedcomIndividualRecord[] people)
        {
            
            var occupationGroups = people
                .Where(p => p.Attributes.Any(a => a.Tag == GedcomIndividualAttributeRecord.OccupationTag))
                .SelectMany(p => p.Attributes
                    .Where(a => a.Tag == GedcomIndividualAttributeRecord.OccupationTag)
                    .GroupBy(a => a.Text)
                    .Select(g => new Entry
                    {
                        Subject = p, 
                        Occupation = g.First(),
                        Count = g.Count(),
                        EarliestDate = g.Min(a=> a.Date),
                        LatestDate = g.Max(a => a.Date)
                    }))
                .GroupBy(e => e.Occupation.Text)
                .OrderBy(g => g.Key);

            foreach (var occupationGroup in occupationGroups)
            {
                var occupationName = occupationGroup.Key;
                writer.WriteLine($"## {occupationName}");
                writer.WriteLine();
                var orderedOccupationGroup = occupationGroup
                    .OrderBy(e => e.Subject.FamilyName)
                    .ThenBy(e => e.Subject.GivenName)
                    .ThenBy(e => e.Subject.BirthEvent?.Date);
                foreach (var person in orderedOccupationGroup)
                {
                    var name = _nameRenderer.RenderNameWithLifespan(person.Subject, linkName: true, boldName: true,
                        familyNameFirst: true);
                    writer.Write($"* {name}");
                    if (person.Count > 1)
                        writer.Write($", mentioned {"time".ToQuantity(person.Count)}");
                    writer.WriteLine(".");
                }

                writer.WriteLine();
            }
        }
    }
}