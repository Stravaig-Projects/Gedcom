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
                return RenderSibling(relationship, includeGenderWherePossible);
            if (relationship.IsParent)
                return RenderParent(relationship, includeGenderWherePossible);
            if (relationship.IsChild)
                return RenderChild(relationship, includeGenderWherePossible);

            return "unrelated";
        }

        private string RenderChild(Relationship relationship, bool includeGenderWherePossible)
        {
            var pedigree = GetPedigree(relationship);

            if (includeGenderWherePossible)
            {
                switch (relationship.Gender)
                {
                    case Gender.Female:
                        return $"{pedigree}daughter";
                    case Gender.Male:
                        return $"{pedigree}son";
                }
            }

            return $"{pedigree}child";
        }

        private static string GetPedigree(Relationship relationship)
        {
            string pedigree = string.Empty;
            switch (relationship.Pedigree)
            {
                case Pedigree.Adopted:
                    pedigree = "adopted-";
                    break;
                case Pedigree.Fostered:
                    pedigree = "foster-";
                    break;
                case Pedigree.Step:
                    pedigree = "step-";
                    break;
            }

            return pedigree;
        }

        private string RenderParent(Relationship relationship, in bool includeGenderWherePossible)
        {
            string pedigree = GetPedigree(relationship);
            
            if (includeGenderWherePossible)
            {
                switch (relationship.Gender)
                {
                    case Gender.Female:
                        return $"{pedigree}mother";
                    case Gender.Male:
                        return $"{pedigree}father";
                }
            }

            return $"{pedigree}parent";
        }

        private string RenderSibling(Relationship relationship, bool includeGenderWherePossible)
        {
            string pedigree = GetPedigree(relationship);
            if (includeGenderWherePossible)
            {
                switch (relationship.Gender)
                {
                    case Gender.Female:
                        return $"{pedigree}sister";
                    case Gender.Male:
                        return $"{pedigree}brother";
                    case Gender.NonBinary:
                        return $"{pedigree}emmer";
                }
            }

            return $"{pedigree}sibling";
        }
    }
}