using System.IO;
using System.Linq;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class OccupationMarkdownRenderer : AttributeRendererBase, IOccupationRenderer
    {
        public OccupationMarkdownRenderer(IDateRenderer dateRenderer) 
            : base(dateRenderer)
        {
        }

        public void WriteOccupations(TextWriter writer, GedcomIndividualRecord subject,
            IStaticFootnoteOrganiser footnoteOrganiser)
        {
            WriteAttribute(writer, subject, footnoteOrganiser);
        }

        protected override GedcomIndividualAttributeRecord[] GetAttributes(GedcomIndividualRecord subject) =>
            subject.Attributes
                .Where(a => a.Tag == GedcomIndividualAttributeRecord.OccupationTag)
                .OrderBy(a => a.Date)
                .ToArray();

        protected override string GetDescription(GedcomIndividualRecord subject, GedcomIndividualAttributeRecord attr)
        {
            string result = attr.Text.HasContent() ? attr.Text : "Unknown";

            if ((attr.Place?.Name).HasContent())
                result += $" in {attr.NormalisedPlaceName()}";

            return result;
        }

        protected override string SingularAttributeName => "Occupation";
        protected override string PluralAttributeName => "Occupations";
    }
}