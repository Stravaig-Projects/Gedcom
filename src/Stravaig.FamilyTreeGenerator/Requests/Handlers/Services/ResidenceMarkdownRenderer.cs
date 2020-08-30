using System.IO;
using System.Linq;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class ResidenceMarkdownRenderer : AttributeRendererBase, IResidenceRenderer
    {
        public ResidenceMarkdownRenderer(IDateRenderer dateRenderer)
            : base(dateRenderer)
        {
        }

        protected override GedcomIndividualAttributeRecord[] GetAttributes(GedcomIndividualRecord subject) =>
            subject.Attributes
                .Where(a => a.Tag == GedcomIndividualAttributeRecord.ResidenceTag)
                .OrderBy(a => a.Date)
                .ToArray();

        protected override string SingularAttributeName => "Residence";
        protected override string PluralAttributeName => "Residences";
        protected override string GetDescription(GedcomIndividualRecord subject, GedcomIndividualAttributeRecord attr)
        {
            if ((attr.Address?.Text).HasContent())
                return attr.Address.Text;
            if (attr.Text.HasContent())
                return attr.Text;
            if ((attr.Place?.Name).HasContent())
                return attr.NormalisedPlaceName();
            return "Unknown";
        }

        public void WriteResidence(
            TextWriter writer, 
            GedcomIndividualRecord subject,
            IStaticFootnoteOrganiser footnoteOrganiser)
        {
            WriteAttribute(writer, subject, footnoteOrganiser);
        }
    }
}