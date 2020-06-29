using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Blazor.Shared.Timeline
{
    public abstract class SubjectEntryBase : EventBase
    {
        protected override EventRecord Event => (EventRecord)Entry.IndividualEvent ?? Entry.IndividualAttribute;

        protected bool IsBorn => Tag == GedcomIndividualEventRecord.BirthTag;
        protected bool IsDead => Tag == GedcomIndividualEventRecord.DeathTag;
        protected bool IsOccupation => Tag == GedcomIndividualAttributeRecord.OccupationTag;
        protected bool IsResidence => Tag == GedcomIndividualAttributeRecord.ResidenceTag;
        protected bool IsBaptism => Tag == GedcomIndividualEventRecord.BaptismTag;
        protected bool IsImmigration => Tag == GedcomIndividualEventRecord.ImmigrationTag;
        protected bool IsNaturalisation => Tag == GedcomIndividualEventRecord.NaturalisationTag;

    }
}