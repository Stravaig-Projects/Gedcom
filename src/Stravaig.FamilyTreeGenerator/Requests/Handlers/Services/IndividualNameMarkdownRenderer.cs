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
            GedcomSourceRecord inRelationToSource,
            bool boldName = false, 
            bool familyNameFirst = false)
        {
            string sourceLocation =
                new FileInfo(_fileNamer.GetSourceFile(inRelationToSource)).Directory?.FullName ?? string.Empty;
            return RenderLinkedNameWithLifespan(subject, sourceLocation, boldName);
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, 
            GedcomIndividualRecord inRelationToPerson, 
            bool boldName = false, 
            bool familyNameFirst = false)
        {
            string sourceLocation =
                new FileInfo(_fileNamer.GetIndividualFile(inRelationToPerson)).Directory?.FullName ?? string.Empty;
            return RenderLinkedNameWithLifespan(subject, sourceLocation, boldName);
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, 
            string linkLocationInRelationTo = null,
            bool boldName = false, 
            bool familyNameFirst = false)
        {
            if (linkLocationInRelationTo == null)
                linkLocationInRelationTo = _fileNamer.BaseDirectory().FullName;

            var filePath = _fileNamer.GetIndividualFile(subject, linkLocationInRelationTo);
            
            StringBuilder sb = new StringBuilder();
            Bold(sb, boldName);
            sb.Append($"[{GetName(subject, familyNameFirst)}]({filePath})");
            Bold(sb, boldName);
            
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

        private static string GetName(GedcomIndividualRecord subject, bool familyNameFirst)
        {
            if (familyNameFirst)
                return $"{subject.FamilyName}, {subject.GivenName}";
            return subject.NameWithoutMarker;
        }

        private void Bold(StringBuilder sb, in bool isBold)
        {
            if (isBold)
                sb.Append("**");
        }
    }
}