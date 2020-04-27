using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IAssociatesOrganiser
    {
        public void AddAssociate(GedcomIndividualRecord associate);
        public GedcomIndividualRecord[] Associates { get; }
    }
}