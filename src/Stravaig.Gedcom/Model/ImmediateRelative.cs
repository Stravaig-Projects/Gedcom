namespace Stravaig.Gedcom.Model
{
    public class ImmediateRelative
    {
        public GedcomIndividualRecord Subject { get; }
        public GedcomIndividualRecord Relative { get; }
        public Relationship TypeOfRelationship { get; }

        public ImmediateRelative(GedcomIndividualRecord subject, GedcomIndividualRecord relative, Relationship typeOfRelationship)
        {
            Subject = subject;
            Relative = relative;
            TypeOfRelationship = typeOfRelationship;
        }

        public override int GetHashCode()
        {
            return Subject.GetHashCode() | Relative.GetHashCode() | TypeOfRelationship.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ImmediateRelative other)
                return this == other;
            
            return false;
        }

        public static bool operator ==(ImmediateRelative a, ImmediateRelative b) =>
            ((object)a == null && (object)b == null) || // cast to object to remove recursive call
            (
                !((object)a == null || (object)b == null) && // cast to object to remove recursive call
                a.Subject == b.Subject &&
                a.Relative == b.Relative &&
                a.TypeOfRelationship == b.TypeOfRelationship
            );

        public static bool operator !=(ImmediateRelative a, ImmediateRelative b) => !(a == b);
    }
}