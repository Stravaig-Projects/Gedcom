using System.IO;
using System.Linq;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public abstract class AttributeRendererBase : TimelineMarkdownRendererBase
    {
        protected AttributeRendererBase(IDateRenderer dateRenderer)
            : base(dateRenderer)
        {
        }

        protected abstract GedcomIndividualAttributeRecord[] GetAttributes(GedcomIndividualRecord subject);
        protected abstract string GetDescription(GedcomIndividualRecord subject,
            GedcomIndividualAttributeRecord attribute);
        protected abstract string SingularAttributeName { get; }
        protected abstract string PluralAttributeName { get; }


        protected void WriteAttribute(TextWriter writer, GedcomIndividualRecord subject,
            IStaticFootnoteOrganiser footnoteOrganiser)
        {
            _writer = writer;
            _footnoteOrganiser = footnoteOrganiser;

            var attributes = GetAttributes(subject);
            if (attributes.Length == 0)
                return;
            
            WriteTable(subject, attributes);
        }
        
        private void WriteTable(GedcomIndividualRecord subject, GedcomIndividualAttributeRecord[] attributes)
        {
            _writer.WriteLine($"## Known {PluralAttributeName}");
            _writer.WriteLine();
            _writer.WriteLine($"Date | {SingularAttributeName} | Sources & Notes");
            _writer.WriteLine("---|---|---");
            foreach (var attribute in attributes)
            {
                var date = attribute.Date != null
                    ? _dateRenderer.RenderAsShortDate(attribute.Date)
                    : "Unknown";
                _writer.Write($"{date} | ");
                var description = GetDescription(subject, attribute);
                _writer.Write($"{description} | ");
                var sources = GetSourceFootnotes(attribute);
                var notes = GetNoteFootnotes(attribute);
                WriteFootnoteLinks(sources.Union(notes));
                _writer.WriteLine();
            }
            _writer.WriteLine();
        }
    }
}