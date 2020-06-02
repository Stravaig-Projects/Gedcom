using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByBirthLocationHandler : RenderPersonIndexBaseHandler
    {
        private readonly IIndividualNameRenderer _nameRenderer;

        public RenderPersonIndexByBirthLocationHandler(
            ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(logger, fileNamer)
        {
            _nameRenderer = nameRenderer;
        }

        protected override string FileName => _fileNamer.GetByBirthLocationIndexFile();

        protected override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
        {
            var countryGroups = people
                .GroupBy(p => GetPlaceName(p)?.LastOrDefault())
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
                    countryGroup.GroupBy(p => GetPlaceName(p)?.TakeWhile(l => l != countryGroup.Key).LastOrDefault());

                foreach (var stateGroup in stateGroups)
                {
                    if (stateGroup.Key.HasContent())
                    {
                        writer.WriteLine($"### {stateGroup.Key}");
                        writer.WriteLine();
                    }

                    var localeGroups = stateGroup
                        .GroupBy(p => string.Join(", ", GetPlaceName(p)?.TakeWhile(l => l != stateGroup.Key) ?? Array.Empty<string>()));
                    foreach (var localeGroup in localeGroups)
                    {
                        if (localeGroup.Key.HasContent())
                        {
                            writer.WriteLine($"#### {localeGroup.Key}");
                            writer.WriteLine();
                        }
                        foreach (var person in localeGroup.OrderByStandardSort())
                        {
                            string linkedName = _nameRenderer.RenderLinkedNameWithLifespan(person, boldName:true, familyNameFirst:true);
                            writer.WriteLine($"- {linkedName}");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        private static string[] GetPlaceName(GedcomIndividualRecord p)
        {
            return p.BirthEvent?.Place?.Name.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }
    }
}