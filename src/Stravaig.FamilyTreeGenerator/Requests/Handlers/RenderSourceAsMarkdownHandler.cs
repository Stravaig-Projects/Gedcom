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
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderSourceAsMarkdownHandler : RequestHandler<RenderSource>
    {
        private readonly ILogger<RenderSourceIndexAsMarkdownHandler> _logger;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IDateRenderer _dateRenderer;
        private readonly IFileNamer _fileNamer;

        public RenderSourceAsMarkdownHandler(
            ILogger<RenderSourceIndexAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IDateRenderer dateRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _nameRenderer = nameRenderer;
            _dateRenderer = dateRenderer;
            _fileNamer = fileNamer;
        }

        public override RenderSource Handle(RenderSource command)
        {
            var sourceEntry = command.SourceEntry;
            var source = sourceEntry.Source;
            
            _logger.LogInformation($"Rendering Source: {source.Title}.");

            var fileName = _fileNamer.GetSourceFile(source);
            _logger.LogInformation($"Writing file to: {fileName}");
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
                    if (source.Notes.Length > 1)
                    {
                        writer.WriteLine($"### Note #{i+1}");
                        writer.WriteLine();
                    }
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
                if (source.IsReferencedByLivingPerson())
                {
                    writer.WriteLine(
                        "_Redacted because this source is referenced by a (potentially) living person and may contain personally identifiable information._");
                    writer.WriteLine();                    
                }
                else
                    writer.WriteMarkdownBlockQuote(source.Text);
            }
        }

        private void WriteHeader(TextWriter writer, GedcomSourceRecord source)
        {
            writer.WriteLine("---");
            writer.WriteLine("layout: page");
            writer.WriteLine($"permalink: /sources/{source.CrossReferenceId.ToString().Trim('@').ToLowerInvariant()}");
            writer.WriteLine("---");
            writer.WriteLine();
            
            writer.WriteLine($"# {source.Title}");
            writer.WriteLine();

            writer.WriteLine("Field | Detail");
            writer.WriteLine("---:|:---");
            writer.WriteLine($"Publication | {source.PublicationFacts}");
            writer.WriteLine($"Originator / Author | {source.Originator}");
            writer.WriteLine($"Date | {_dateRenderer.RenderAsShortDate(source.Date)}");
            writer.WriteLine($"Responsible Agency | {source.ResponsibleAgency}");
            writer.WriteLine($"Filed by Entry | {source.FiledByEntry}");

            writer.Write("References | ");
            if (source.References.Length > 1)
            {
                writer.Write("<ul>");
                foreach (var reference in source.References)
                {
                    writer.Write("<li>");
                    WriteReference(writer, reference);
                    writer.Write("</li>");
                }
                writer.Write("</ul>");
            }
            else if (source.References.Length == 1)
            {
                WriteReference(writer, source.References[0]);                
            }
            writer.WriteLine();
            writer.WriteLine();
        }

        private static void WriteReference(TextWriter writer, GedcomUserReferenceNumberRecord reference)
        {
            if (reference.Type.HasContent())
                writer.Write($"({reference.Type}) ");
            writer.Write(reference.Reference);
        }
    }
}