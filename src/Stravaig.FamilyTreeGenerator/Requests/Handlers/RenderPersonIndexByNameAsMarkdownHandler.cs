using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByNameAsMarkdownHandler : RequestHandler<RenderPersonIndex>
    {
        private readonly ILogger<RenderPersonIndexByNameAsMarkdownHandler> _logger;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IFileNamer _fileNamer;

        public RenderPersonIndexByNameAsMarkdownHandler(
            ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _nameRenderer = nameRenderer;
            _fileNamer = fileNamer;
        }
        public override RenderPersonIndex Handle(RenderPersonIndex command)
        {
            _logger.LogInformation("Rendering Index by name.");

            var fileName = _fileNamer.GetByNameIndexFile();
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer);
            WriteIndex(writer, command.Individuals);
            
            return base.Handle(command);
        }

        private void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("---");
            writer.WriteLine("layout: page");
            writer.WriteLine("permalink: /indexes/by-person-family-name");
            writer.WriteLine("---");
            writer.WriteLine();

            writer.WriteLine("# Index - By Family Name");
        }

        private void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
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
}