using System;
using System.IO;
using System.Linq;
using Humanizer;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByOccupationHandler : RenderPersonIndexBaseHandler
    {
        private readonly IIndividualNameRenderer _nameRenderer;

        private readonly IOccupationDescriptionService _occupationService;

        public class Entry
        {
            public GedcomIndividualRecord Subject;
            public GedcomIndividualAttributeRecord Occupation;
            public string[] TopLevelOccupationNames;
            public int Count;
            public GedcomDateRecord EarliestDate;
            public GedcomDateRecord LatestDate;
        }
        public RenderPersonIndexByOccupationHandler(ILogger<RenderPersonIndexByOccupationHandler> logger, 
            IIndividualNameRenderer nameRenderer,
            IOccupationDescriptionService occupationService,
            IFileNamer fileNamer) 
            : base(logger, fileNamer)
        {
            _nameRenderer = nameRenderer;
            _occupationService = occupationService;
        }

        protected override string FileName => _fileNamer.GetByOccupationIndexFile();
        protected override void WriteIndex(TextWriter writer, 
            GedcomIndividualRecord[] people)
        {
            var occupationGroups = GetOccupationGroups(people);

            var topLevelOccupationGroupNames = occupationGroups
                .SelectMany(g => g.First().TopLevelOccupationNames)
                .Distinct()
                .OrderBy(n => n);

            foreach (var topLevelName in topLevelOccupationGroupNames)
            {
                writer.WriteLine($"## {topLevelName}");
                writer.WriteLine();

                var associatedOccupationGroups = occupationGroups
                    .Where(g => g.First().TopLevelOccupationNames.Contains(topLevelName))
                    .OrderBy(g => g.Key.Equals(topLevelName, StringComparison.InvariantCultureIgnoreCase) ? string.Empty : g.Key)
                    .ToArray();

                int numPeople = associatedOccupationGroups
                    .SelectMany(tg => tg)
                    .Select(og => og.Subject)
                    .Distinct()
                    .Count();
                
                var description = _occupationService.GetDescription(topLevelName);
                if (!string.IsNullOrWhiteSpace(description))
                {
                    writer.WriteLine(description);
                    writer.WriteLine();
                }

                if (numPeople > 1)
                {
                    writer.Write($"There were {numPeople} people who worked as a {topLevelName}");
                    if (associatedOccupationGroups.Length > 1)
                        writer.Write(" of some sort");
                    writer.WriteLine(".");
                    writer.WriteLine();
                }
           
                foreach (var occupationGroup in associatedOccupationGroups)
                {
                    var occupationName = occupationGroup.Key;
                    if (!occupationName.Equals(topLevelName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        writer.WriteLine($"#### {occupationName}");
                        writer.WriteLine();
                    }
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

        private static IGrouping<string, Entry>[] GetOccupationGroups(GedcomIndividualRecord[] people)
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
                        TopLevelOccupationNames = g.First().TopLevelOccupationNames(),
                        Count = g.Count(),
                        EarliestDate = g.Min(a => a.Date),
                        LatestDate = g.Max(a => a.Date)
                    }))
                .GroupBy(e => e.Occupation.Text)
                .ToArray();
            return occupationGroups;
        }
    }
}