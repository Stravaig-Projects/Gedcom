using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IRelationshipRenderer
    {
        string HumanReadable(Relationship relationship, bool includeGenderWherePossible);
    }
}