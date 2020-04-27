using System.IO;
using System.Text;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class IndividualNameMarkdownRenderer : IIndividualNameRenderer
    {
        private readonly IFileNamer _fileNamer;
        private readonly IDateRenderer _dateRenderer;

        public IndividualNameMarkdownRenderer(IFileNamer fileNamer, IDateRenderer dateRenderer)
        {
            _fileNamer = fileNamer;
            _dateRenderer = dateRenderer;
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject,
            GedcomSourceRecord inRelationToSource)
        {
            string sourceLoction =
                new FileInfo(_fileNamer.GetSourceFile(inRelationToSource)).Directory?.FullName ?? string.Empty;
            return RenderLinkedNameWithLifespan(subject, sourceLoction);
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, GedcomIndividualRecord inRelationToPerson)
        {
            string sourceLoction =
                new FileInfo(_fileNamer.GetIndividualFile(inRelationToPerson)).Directory?.FullName ?? string.Empty;
            return RenderLinkedNameWithLifespan(subject, sourceLoction);
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, string linkLocationInRelationTo = null)
        {
            if (linkLocationInRelationTo == null)
                linkLocationInRelationTo = _fileNamer.BaseDirectory().FullName;

            var filePath = _fileNamer.GetIndividualFile(subject, linkLocationInRelationTo);
            
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{subject.NameWithoutMarker}]({filePath})");
            
            if (subject.BirthEvent?.Date != null || subject.DeathEvent?.Date != null)
            {
                sb.Append(" (");
                if (subject.BirthEvent?.Date != null)
                {
                    var birth = _dateRenderer.RenderAsShortDate(subject.BirthEvent.Date);
                    sb.Append($"{birth}");
                }
                sb.Append(" - ");
                if (subject.DeathEvent?.Date != null)
                {
                    var death = _dateRenderer.RenderAsShortDate(subject.DeathEvent.Date);
                    sb.Append($"{death}");
                }
                sb.Append(")");
            }

            return sb.ToString();
        }
    }
}