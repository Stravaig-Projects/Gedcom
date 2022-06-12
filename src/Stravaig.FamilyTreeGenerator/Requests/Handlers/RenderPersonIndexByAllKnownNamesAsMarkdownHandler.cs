using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

// Instantiated by Paramore Brighter
// ReSharper disable once ClassNeverInstantiated.Global
public class RenderPersonIndexByAllKnownNamesAsMarkdownHandler : RenderPersonIndexBaseHandler
{
    private readonly IIndividualNameRenderer _nameRenderer;

    public RenderPersonIndexByAllKnownNamesAsMarkdownHandler(
        ILogger<RenderPersonIndexByAllKnownNamesAsMarkdownHandler> logger,
        IIndividualNameRenderer nameRenderer,
        IFileNamer fileNamer)
        : base(logger, fileNamer)
    {
        _nameRenderer = nameRenderer;
    }

    protected override string FileName => _fileNamer.GetByAllNamesIndexFile();

    protected  override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
    {
        var familyGroups = people
            .SelectMany(p => p.Names.Select(n => new {Person = p, Name = n}))
            .GroupBy(pn => pn.Name.Surname)
            .OrderBy(g => g.Key)
            .ToArray();

        foreach (var family in familyGroups)
        {
            writer.WriteLine();
            writer.WriteLine(string.IsNullOrWhiteSpace(family.Key)
                ? "## ???"
                : $"## {family.Key}");
            writer.WriteLine();
            
            var familyMembers = family
                .OrderBy(pn => pn.Name.Surname)
                .ThenBy(pn => pn.Name.WholeName)
                .ThenBy(pn => pn.Person.BirthEvent?.Date)
                .ToArray();

            foreach (var personName in familyMembers)
            {
                string linkedName = _nameRenderer.RenderLinkedNameWithLifespan(personName.Person, boldName:true, specificName: personName.Name);
                writer.WriteLine($"- {linkedName}");
            }
        }
    }
}