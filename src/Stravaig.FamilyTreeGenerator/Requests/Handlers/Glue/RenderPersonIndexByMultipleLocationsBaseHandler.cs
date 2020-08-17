using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public abstract class RenderPersonIndexByMultipleLocationsBaseHandler : RenderPersonIndexBaseHandler
    {
        private class PersonLocation
        {
            public GedcomIndividualRecord Person { get; set; }
            public string[] Location { get; set; }
        }
        
        private readonly IIndividualNameRenderer _nameRenderer;
        protected RenderPersonIndexByMultipleLocationsBaseHandler(
            ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(logger, fileNamer)
        {
            _nameRenderer = nameRenderer;
        }

        protected override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
        {
            var countryGroups = people
                .SelectMany(GetLocations)
                .GroupBy(pl => pl.Location.LastOrDefault())
                .OrderBy(g => g.Key)
                .ToArray();

            foreach (var countryGroup in countryGroups)
            {
                writer.WriteLine();
                if (countryGroup.Key.HasContent())
                    writer.WriteLine($"## {countryGroup.Key}");
                else
                    writer.WriteLine("## _Unknown_");
                writer.WriteLine();
                var stateGroups =
                    countryGroup.GroupBy(pl => pl.Location.TakeWhile(l => l != countryGroup.Key).LastOrDefault())
                        .OrderBy(g => g.Key);

                foreach (var stateGroup in stateGroups)
                {
                    if (stateGroup.Key.HasContent())
                    {
                        writer.WriteLine($"### {stateGroup.Key}");
                        writer.WriteLine();
                    }

                    var localeGroups = stateGroup
                        .GroupBy(pl => string.Join(", ",
                            pl.Location.TakeWhile(l => l != stateGroup.Key) ?? Array.Empty<string>()))
                        .OrderBy(g => g.Key);
                    foreach (var localeGroup in localeGroups)
                    {
                        if (localeGroup.Key.HasContent())
                        {
                            writer.WriteLine($"#### {localeGroup.Key}");
                            writer.WriteLine();
                        }
                        foreach (var person in localeGroup.Select(pl => pl.Person).OrderByStandardSort())
                        {
                            string linkedName = _nameRenderer.RenderLinkedNameWithLifespan(person, boldName:true, familyNameFirst:true);
                            writer.WriteLine($"- {linkedName}");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        private IEnumerable<PersonLocation> GetLocations(GedcomIndividualRecord person)
        {
            var places = GetPlaceNames(person);
            foreach (var place in places)
                yield return new PersonLocation
                {
                    Person = person, 
                    Location = place
                };
        }

        protected abstract string[][] GetPlaceNames(GedcomIndividualRecord person);
    }
}