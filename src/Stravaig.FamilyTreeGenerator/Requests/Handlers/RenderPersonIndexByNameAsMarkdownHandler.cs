using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

// Instantiated by Paramore Brighter
// ReSharper disable once ClassNeverInstantiated.Global
public class RenderPersonIndexByNameAsMarkdownHandler : RenderPersonIndexBaseHandler
{
    private readonly IIndividualNameRenderer _nameRenderer;

    public RenderPersonIndexByNameAsMarkdownHandler(
        ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
        IIndividualNameRenderer nameRenderer,
        IFileNamer fileNamer)
        : base(logger, fileNamer)
    {
        _nameRenderer = nameRenderer;
    }

    protected override string FileName => _fileNamer.GetByNameIndexFile();

    protected  override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
    {
        var familyGroups = people
            .GroupBy(p => p.FamilyName)
            .OrderBy(g => g.Key)
            .ToArray();

        foreach (var family in familyGroups)
        {
            writer.WriteLine();
            if (string.IsNullOrWhiteSpace(family.Key))
                writer.WriteLine("## ???");
            else
                writer.WriteLine($"## {family.Key}");
            writer.WriteLine();

                
            var familyMembers = family
                .OrderByStandardSort()
                .ToArray();
            foreach (var person in familyMembers)
            {
                string linkedName = _nameRenderer.RenderLinkedNameWithLifespan(person, boldName:true);
                writer.Write($"- {linkedName}");
                writer.WriteLine();
            }
        }
    }
}