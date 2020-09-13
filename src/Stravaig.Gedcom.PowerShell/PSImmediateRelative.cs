using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.PowerShell.Extensions;

namespace Stravaig.Gedcom.PowerShell
{
    // ReSharper disable once InconsistentNaming
    public class PSImmediateRelative
    {
        private readonly ImmediateRelative _immediateRelative;

        public PSImmediateRelative(ImmediateRelative immediateRelative)
        {
            _immediateRelative = immediateRelative;
        }

        public PSGedcomIndividual Subject => _immediateRelative.Subject.Wrap();
        public PSGedcomIndividual Relative => _immediateRelative.Relative.Wrap();
        public Relationship TypeOfRelationship => _immediateRelative.TypeOfRelationship;

        public override string ToString()
        {
            string tor;
            if (TypeOfRelationship.IsParent)
                tor = "parent";
            else if (TypeOfRelationship.IsChild)
                tor = "child";
            else if (TypeOfRelationship.IsSibling)
                tor = "sibling";
            else if (TypeOfRelationship.IsSpouse)
                tor = "spouse/partner";
            else if (TypeOfRelationship.IsSelf)
                tor = "self";
            else
                tor = "unknown/too complex";

            return $"{Relative} ({tor})";
        }
    }
}