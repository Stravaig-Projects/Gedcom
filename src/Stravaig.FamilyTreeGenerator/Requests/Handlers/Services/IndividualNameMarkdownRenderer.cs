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
            bool familyNameFirst = false,
            GedcomNameRecord specificName = null)
        {
            string sourceLocation =
                new FileInfo(_fileNamer.GetSourceFile(inRelationToSource)).Directory?.FullName ?? string.Empty;
            return RenderLinkedNameWithLifespan(subject, sourceLocation, boldName, familyNameFirst, specificName);
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, 
            GedcomIndividualRecord inRelationToPerson, 
            bool boldName = false, 
            bool familyNameFirst = false,
            GedcomNameRecord specificName = null)
        {
            string sourceLocation =
                new FileInfo(_fileNamer.GetIndividualFile(inRelationToPerson)).Directory?.FullName ?? string.Empty;
            return RenderLinkedNameWithLifespan(subject, sourceLocation, boldName, familyNameFirst, specificName);
        }

        public string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject,
            string linkLocationInRelationTo = null,
            bool boldName = false,
            bool familyNameFirst = false,
            GedcomNameRecord specificName = null)
        {
            return RenderNameWithLifespan(subject,
                linkName: true,
                linkLocationInRelationTo: linkLocationInRelationTo,
                boldName: boldName,
                familyNameFirst: familyNameFirst,
                specificName: specificName);
        }
        
        public string RenderNameWithLifespan(GedcomIndividualRecord subject, 
            bool linkName = false,
            string linkLocationInRelationTo = null,
            bool boldName = false, 
            bool familyNameFirst = false,
            GedcomNameRecord specificName = null)
        {
            string filePath = string.Empty;
            if (linkName)
            {
                if (linkLocationInRelationTo == null)
                    linkLocationInRelationTo = _fileNamer.BaseDirectory().FullName;

                filePath = _fileNamer.GetIndividualFile(subject, linkLocationInRelationTo);
            }
            
            StringBuilder sb = new StringBuilder();
            Bold(sb, boldName);
            if (linkName)
                sb.Append($"[{GetName(subject, familyNameFirst, specificName)}]({filePath})");
            else
                sb.Append(GetName(subject, familyNameFirst, specificName));
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

        private static string GetName(GedcomIndividualRecord subject, bool familyNameFirst, GedcomNameRecord specificName)
        {
            if (specificName == null)
            {
                if (familyNameFirst)
                    return $"{subject.FamilyName}, {subject.GivenName}";
                return subject.NameWithoutMarker;
            }

            if (familyNameFirst)
                return $"{specificName.Surname}, {specificName.Name}";
            return specificName.WholeName;
        }

        private void Bold(StringBuilder sb, in bool isBold)
        {
            if (isBold)
                sb.Append("**");
        }
    }
}