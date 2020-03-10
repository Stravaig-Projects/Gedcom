using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderIndividual : Request
    {
        public GedcomIndividualRecord Individual { get; }

        public RenderIndividual(GedcomIndividualRecord individual)
        {
            Individual = individual;
        }
    }
}