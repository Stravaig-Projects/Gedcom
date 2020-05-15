using System.IO;
using System.Linq;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class MarkdownFootnoteOrganiser : FootnoteOrganiser
    {
        private readonly IDateRenderer _dateRenderer;
        private readonly IFileNamer _fileNamer;

        public MarkdownFootnoteOrganiser(IDateRenderer dateRenderer, IFileNamer fileNamer)
        {
            _dateRenderer = dateRenderer;
            _fileNamer = fileNamer;
        }

        public override void WriteFootnotes(TextWriter writer, GedcomIndividualRecord linkRelativeTo)
        {
            var dir = new FileInfo(_fileNamer.GetIndividualFile(linkRelativeTo)).Directory?.FullName;
            WriteFootnotes(writer, dir);
        }

        public override void WriteFootnotes(TextWriter writer, string linkRelativeTo)
        {
            if (_footnotes.Any())
            {
                writer.WriteLine("## Footnotes");
                writer.WriteLine();
                foreach (var footnote in _footnotes.OrderBy(f=>f.Id))
                {
                    writer.WriteLine($"### {footnote.Id}");
                    writer.WriteLine();
                    if (footnote.Note != null)
                        writer.WriteMarkdownBlockQuote(footnote.Note.Text);
                    else
                        WriteSource(writer, footnote, linkRelativeTo);
                    writer.WriteLine();
                }
            }
        }

        private void WriteSource(TextWriter writer, Footnote footnote, string linkRelativeTo)
        {
            var source = footnote.Source;
            if (source != null)
            {
                _addSource(source, _subject);
                if (source.Title.HasContent())
                {
                    writer.WriteLine($"**{source.Title}**");
                    writer.WriteLine();
                }

                if (source.Text.HasContent() || source.Notes.Any())
                {
                    string sourceFile = _fileNamer.GetSourceFile(source, linkRelativeTo);
                    writer.WriteLine($"* [Full text and notes]({sourceFile})");
                }

                if (source.PublicationFacts.HasContent())
                    writer.WriteLine($"* Publication: {source.PublicationFacts.RenderLinksAsMarkdown()}");
                
                if (source.Originator.HasContent())
                    writer.WriteLine($"* Originator / Author: {source.Originator}");

                if (source.Date != null)
                    writer.WriteLine($"* Date: {_dateRenderer.RenderAsShortDate(source.Date)}");

                if (source.ResponsibleAgency.HasContent())
                    writer.WriteLine($"* Responsible Agency: {source.ResponsibleAgency}");

                if (source.FiledByEntry.HasContent())
                    writer.WriteLine($"* Filed by Entry: {source.FiledByEntry}");

                if (source.References.Any(r => r.Reference.HasContent()))
                {
                    writer.WriteLine("* References: ");
                    foreach (var reference in source.References)
                    {
                        writer.Write($"  * ");
                        if (reference.Type.HasContent())
                            writer.Write($"({reference.Type}) ");
                        writer.WriteLine(reference.Reference);
                    }
                }
            }
        }
    }
}