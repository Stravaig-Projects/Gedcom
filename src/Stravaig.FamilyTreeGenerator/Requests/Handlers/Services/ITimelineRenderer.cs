using System.IO;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface ITimelineRenderer
    {
        void WriteTimeline(TextWriter writer, GedcomIndividualRecord subject, IStaticFootnoteOrganiser footnoteOrganiser,
            IAssociatesOrganiser associatesOrganiser);
    }
}