using System.Linq;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class RelationshipExtensions
    {
        public static Relationship GetRelationshipTo(this GedcomIndividualRecord subject,
            GedcomIndividualRecord potentialRelation)
        {
            if (subject.CrossReferenceId == potentialRelation.CrossReferenceId)
                return new Relationship(subject.Sex.ToGender(), GenerationZeroRelationships.Self);

            var parent = subject.ChildToFamilies
                .SelectMany(f => f.Spouses)
                .FirstOrDefault(p => p.CrossReferenceId == potentialRelation.CrossReferenceId);
            if (parent != null)
            {
                Gender gender = parent.Sex.ToGender();
                return new Relationship(gender, 1, Direction.Ancestor);
            }

            var sibling = subject.ChildToFamilies
                .SelectMany(f => f.Children)
                .FirstOrDefault(s => s.CrossReferenceId == potentialRelation.CrossReferenceId);
            if (sibling != null)
            {
                Gender gender = sibling.Sex.ToGender();
                return new Relationship(gender, GenerationZeroRelationships.Sibling);
            }

            
            var spouse = subject.SpouseToFamilies
                .SelectMany(s => s.Spouses)
                .FirstOrDefault(s => s.CrossReferenceId == potentialRelation.CrossReferenceId);
            if (spouse != null)
            {
                Gender gender = spouse.Sex.ToGender();
                return new Relationship(gender, GenerationZeroRelationships.Spouse);
            }

            var child = subject.SpouseToFamilies
                .SelectMany(s => s.Children)
                .FirstOrDefault(c => c.CrossReferenceId == potentialRelation.CrossReferenceId);
            if (child != null)
            {
                Gender gender = child.Sex.ToGender();
                return new Relationship(gender, 1, Direction.Descendent);
            }

            return Relationship.NotRelated;
        }
    }
}