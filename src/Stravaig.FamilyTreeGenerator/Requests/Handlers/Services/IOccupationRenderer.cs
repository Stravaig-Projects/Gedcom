using System.IO;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IOccupationRenderer
    {
        void WriteOccupations(TextWriter writer, GedcomIndividualRecord subject,
            IStaticFootnoteOrganiser footnoteOrganiser);
    }
}