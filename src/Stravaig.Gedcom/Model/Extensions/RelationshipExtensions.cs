using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

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

        public static Qualification? GetPedigree(this GedcomIndividualRecord child, GedcomIndividualRecord parent)
        {
            // If there is no child/parent relationship this will return null
            var pedigree = child.FamilyLinks
                .Where(fl => fl.Type == GedcomFamilyType.ChildToFamily)
                .Where(fl => fl.Family.Spouses.Any(s => s == parent))
                .Select(fl => fl.Qualification)
                .OrderBy(p => p)
                .FirstOrDefault();
            return pedigree;
        }
        
        public static ImmediateRelative[] GetImmediateRelatives(this GedcomIndividualRecord subject)
        {
            var relatives = GetAllImmediateRelatives(subject)
                .Where(r => r.Relative != subject)
                .GroupBy(ir => ir.Relative)
                .Select(g => g.First())
                .ToArray();
            return relatives;
         }

        public static ImmediateRelative[] Parents(this GedcomIndividualRecord subject)
        {
            return GetImmediateRelatives(subject)
                .Where(ir => ir.TypeOfRelationship.IsParent)
                .ToArray();
        }
        
        public static ImmediateRelative[] Children(this GedcomIndividualRecord subject)
        {
            return GetImmediateRelatives(subject)
                .Where(ir => ir.TypeOfRelationship.IsChild)
                .ToArray();
        }

        private static IEnumerable<ImmediateRelative> GetAllImmediateRelatives(GedcomIndividualRecord subject)
        {
            foreach (var link in subject.FamilyLinks)
            {
                var family = link.Family;
                if (link.Type == GedcomFamilyType.SpouseToFamily)
                {
                    foreach (var spouse in family.Spouses.Where(s => s != subject))
                    {
                        bool isMarried = family.Events.Any(fe => fe.Tag == GedcomFamilyEventRecord.MarriageTag);
                        bool isDivorced = family.Events.Any(fe =>
                            fe.Tag == GedcomFamilyEventRecord.AnnulmentTag ||
                            fe.Tag == GedcomFamilyEventRecord.DivorceTag);
                        Qualification spousalRelationship = isMarried
                            ? isDivorced
                                ? Qualification.Ex
                                : Qualification.Married
                            : Qualification.Unknown;
                        yield return new ImmediateRelative(subject, spouse, new Relationship(spouse.Sex.ToGender(), GenerationZeroRelationships.Spouse, spousalRelationship));
                    }

                    foreach (var child in family.Children)
                    {
                        Qualification qualification = child.GetPedigree(subject) ?? Qualification.Unknown;
                        yield return new ImmediateRelative(subject, child, new Relationship(child.Sex.ToGender(), 1, Direction.Descendent, qualification));
                    }
                }
                else if (link.Type == GedcomFamilyType.ChildToFamily)
                {
                    foreach (var parent in family.Spouses)
                    {
                        Qualification qualification = subject.GetPedigree(parent) ?? Qualification.Unknown;
                        yield return new ImmediateRelative(subject, parent, new Relationship(parent.Sex.ToGender(), 1, Direction.Ancestor, qualification));
                    }

                    foreach (var sibling in family.Children)
                    {
                        yield return new ImmediateRelative(subject, sibling, new Relationship(sibling.Sex.ToGender(), GenerationZeroRelationships.Sibling));
                    }
                }
            }
        }
    }
}