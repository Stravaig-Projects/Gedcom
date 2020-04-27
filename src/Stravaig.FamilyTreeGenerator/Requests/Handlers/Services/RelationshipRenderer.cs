using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class RelationshipRenderer : IRelationshipRenderer
    {
        
        public string HumanReadable(Relationship relationship, bool includeGenderWherePossible)
        {
            if (relationship.IsNotRelated)
                return "unrelated";
            if (relationship.IsSelf)
                return "self";
            if (relationship.IsSpouse)
                return "spouse";
            if (relationship.IsSibling)
                return RenderSibling(relationship.Gender, includeGenderWherePossible);
            if (relationship.IsParent)
                return RenderParent(relationship.Gender, includeGenderWherePossible);
            if (relationship.IsChild)
                return RenderChild(relationship.Gender, includeGenderWherePossible);

            return "unrelated";
        }

        private string RenderChild(Gender gender, in bool includeGenderWherePossible)
        {
            if (includeGenderWherePossible)
            {
                switch (gender)
                {
                    case Gender.Female:
                        return "daughter";
                    case Gender.Male:
                        return "son";
                }
            }

            return "child";
        }

        private string RenderParent(Gender gender, in bool includeGenderWherePossible)
        {
            if (includeGenderWherePossible)
            {
                switch (gender)
                {
                    case Gender.Female:
                        return "mother";
                    case Gender.Male:
                        return "father";
                }
            }

            return "parent";
        }

        private string RenderSibling(Gender gender, bool includeGenderWherePossible)
        {
            if (includeGenderWherePossible)
            {
                switch (gender)
                {
                    case Gender.Female:
                        return "sister";
                    case Gender.Male:
                        return "brother";
                    case Gender.NonBinary:
                        return "emmer";
                }
            }

            return "sibling";
        }
    }
}