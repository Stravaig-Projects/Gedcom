using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Common.Humaniser
{
    public interface IRelationshipRenderer
    {
        string HumanReadable(Relationship relationship, bool includeGenderWherePossible);
    }
}