using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderIndex : Request
    {
        public GedcomIndividualRecord[] Individuals { get; }

        public RenderIndex(GedcomIndividualRecord[] individuals)
        {
            Individuals = individuals;
        }
    }
}