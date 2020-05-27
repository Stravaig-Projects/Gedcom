namespace Stravaig.Gedcom.Model
{
    public enum Qualification
    {
        // These are ordered with the most prominent at the top in case a
        // person finds themselves in multiple families with the same parent.
        // e.g. Parents get divorced, and one gets remarried. The child may
        // have a pedigree attached to the second family as "Adopted", but one
        // of the parents is still the biological parent. The file does not
        // recognise this family structure directly, but it can be inferred by
        // looking at other families a child is a part of.
        // The biological relationship is considered the strongest.
        Biological,
        Adopted,
        Fostered,
        Sealed,
        Step, // Non-standard. Used by MobileFamilyTree
        Married, // Non-standard. Used to note spousal relationships.
        Ex, // Non-standard. Used to note spousal relationships that have ended.
        Unknown, // Non-standard. Not biological, but relationship exists.
    }
}