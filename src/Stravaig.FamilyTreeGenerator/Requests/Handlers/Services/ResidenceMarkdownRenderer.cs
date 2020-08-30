using System.IO;
using System.Linq;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class ResidenceMarkdownRenderer : TimelineMarkdownRendererBase, IResidenceRenderer
    {
        private readonly IDateRenderer _dateRenderer;

        public ResidenceMarkdownRenderer(IDateRenderer dateRenderer)
        {
            _dateRenderer = dateRenderer;
        }

        public void WriteResidence(
            TextWriter writer, 
            GedcomIndividualRecord subject,
            IStaticFootnoteOrganiser footnoteOrganiser)
        {
            _writer = writer;
            _footnoteOrganiser = footnoteOrganiser;

            var residences = subject.Attributes
                .Where(a => a.Tag == GedcomIndividualAttributeRecord.ResidenceTag)
                .OrderBy(a => a.Date)
                .ToArray();

            if (residences.Length == 0)
                return;
            
            WriteHeader(subject, residences);
            
        }

        private void WriteHeader(GedcomIndividualRecord subject, GedcomIndividualAttributeRecord[] residences)
        {
            _writer.WriteLine("## Known Residences");
            _writer.WriteLine();
            _writer.WriteLine("Date | Residence | Sources & Notes");
            _writer.WriteLine("---|---|---");
            foreach (var residence in residences)
            {
                var date = residence.Date != null
                    ? _dateRenderer.RenderAsShortDate(residence.Date)
                    : "Unknown";
                _writer.Write($"{date} | ");
                var location = GetResidence(residence);
                _writer.Write($"{location} | ");
                var sources = GetSourceFootnotes(residence);
                var notes = GetNoteFootnotes(residence);
                WriteFootnoteLinks(sources.Union(notes));
                _writer.WriteLine();
            }
            _writer.WriteLine();
        }
        
        private string GetResidence(GedcomIndividualAttributeRecord attr)
        {
            if ((attr.Address?.Text).HasContent())
                return attr.Address.Text;
            if (attr.Text.HasContent())
                return attr.Text;
            if ((attr.Place?.Name).HasContent())
                return attr.NormalisedPlaceName();
            return "Unknown";
        }
    }
}