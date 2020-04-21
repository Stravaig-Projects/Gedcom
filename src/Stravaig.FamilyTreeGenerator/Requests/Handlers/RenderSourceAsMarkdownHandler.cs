using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Requests.Models;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderSourceAsMarkdownHandler : RequestHandler<RenderSource>
    {
        private readonly ILogger<RenderSourceIndexAsMarkdownHandler> _logger;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IFileNamer _fileNamer;

        public RenderSourceAsMarkdownHandler(
            ILogger<RenderSourceIndexAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _nameRenderer = nameRenderer;
            _fileNamer = fileNamer;
        }

        public override RenderSource Handle(RenderSource command)
        {
            var sourceEntry = command.SourceEntry;
            var source = sourceEntry.Source;
            
            _logger.LogInformation($"Rendering Source: {source.Title}.");

            var fileName = _fileNamer.GetSourceFile(source);
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);

            WriteHeader(writer, source);
            WriteSourceText(writer, source);
            WriteNotes(writer, source);
            WriteReferencedBy(writer, sourceEntry);
            return base.Handle(command);
        }

        private void WriteReferencedBy(TextWriter writer, SourceEntry entry)
        {
            var subjects = entry.ReferencedByIndividuals;
            if (subjects.Any())
            {
                writer.WriteLine("## Source Referenced by");
                writer.WriteLine();
                var orderedSubjects = subjects.OrderByStandardSort();
                foreach (var subject in orderedSubjects)
                {
                    string name = _nameRenderer.RenderLinkedNameWithLifespan(subject, entry.Source);
                    writer.WriteLine($"* {name}");
                }
            }
        }

        private void WriteNotes(TextWriter writer, GedcomSourceRecord source)
        {
            if (source.Notes.Any())
            {
                writer.WriteLine("## Notes");
                writer.WriteLine();
                for(int i = 0; i < source.Notes.Length; i++)
                {
                    writer.WriteLine($"### Note #{i}");
                    writer.WriteLine();
                    writer.WriteMarkdownBlockQuote(source.Notes[i].Text);
                    writer.WriteLine();
                }
            }
        }

        private void WriteSourceText(TextWriter writer, GedcomSourceRecord source)
        {
            if (source.Text.HasContent())
            {
                writer.WriteLine("## Text");
                writer.WriteLine();
                writer.WriteMarkdownBlockQuote(source.Text);
            }
        }

        private void WriteHeader(TextWriter writer, GedcomSourceRecord source)
        {
            writer.WriteLine($"# {source.Title}");
            writer.WriteLine();

            writer.WriteLine("Name | Value");
            writer.WriteLine("---:|:---");
            writer.WriteLine($"Responsible Agency | {source.ResponsibleAgency}");
            writer.WriteLine($"Source Originator | {source.Originator}");
            writer.WriteLine($"Source Publication Facts | {source.PublicationFacts}");
            writer.WriteLine();
        }
    }
}