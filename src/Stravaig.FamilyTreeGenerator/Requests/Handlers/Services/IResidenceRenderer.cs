using System.IO;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IResidenceRenderer
    {
        void WriteResidence(
            TextWriter writer,
            GedcomIndividualRecord subject,
            IStaticFootnoteOrganiser footnoteOrganiser);
    }
}