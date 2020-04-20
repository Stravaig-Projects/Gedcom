using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderPersonIndex : Request
    {
        public GedcomIndividualRecord[] Individuals { get; }

        public RenderPersonIndex(GedcomIndividualRecord[] individuals)
        {
            Individuals = individuals;
        }
    }
}