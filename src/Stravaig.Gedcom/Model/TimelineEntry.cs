using System;
using System.Diagnostics;
using System.Text;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class TimelineEntry
    {
        public enum EventType
        {
            SubjectEvent,
            FamilyEvent,
            FamilyMemberEvent,
        }
        
        private readonly Func<GedcomDateRecord> _getDate;
        public GedcomIndividualRecord Subject { get; }
        public GedcomIndividualRecord OtherFamilyMember { get; }
        public GedcomIndividualEventRecord IndividualEvent { get; }
        
        public GedcomFamilyRecord Family { get; }
        public GedcomFamilyEventRecord FamilyEvent { get; }
        
        public GedcomDateRecord Date => _getDate();
        public EventType Type { get; }
        public TimelineEntry(GedcomIndividualRecord subject, GedcomIndividualEventRecord individualEvent)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            IndividualEvent = individualEvent ?? throw new ArgumentNullException(nameof(individualEvent));
            _getDate = GetIndividualEventDate;
            Type = EventType.SubjectEvent;
        }

        public TimelineEntry(GedcomIndividualRecord subject, GedcomIndividualRecord otherFamilyMember, GedcomIndividualEventRecord individualEvent)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            OtherFamilyMember = otherFamilyMember ?? throw new ArgumentNullException(nameof(otherFamilyMember));
            IndividualEvent = individualEvent ?? throw new ArgumentNullException(nameof(individualEvent));
            _getDate = GetIndividualEventDate;
            Type = EventType.FamilyMemberEvent;
        }

        public TimelineEntry(GedcomIndividualRecord subject, GedcomFamilyRecord family,
            GedcomFamilyEventRecord familyEvent)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Family = family ?? throw new ArgumentNullException(nameof(family));
            FamilyEvent = familyEvent ?? throw new ArgumentNullException(nameof(familyEvent));
            _getDate = GetFamilyEventDate;
            Type = EventType.FamilyEvent;
        }

        public override string ToString()
        {
            //[DebuggerDisplay("{Type} {Subject.Name} {Date.RawDateValue} {IndividualEvent?.Tag}:{IndividualEvent?.Type} {FamilyEvent?.Tag}:{FamilyEvent?.Type} {OtherFamilyMember?.Name}")]
            StringBuilder sb = new StringBuilder();
            sb.Append($"Timeline:{Type} {Subject.Name} {Date.RawDateValue}");
            EventRecord @event = IndividualEvent ?? (EventRecord)FamilyEvent;
            if (@event != null)
            {
                sb.Append(" ");
                sb.Append(@event.Tag);
                if (@event.Type.HasContent())
                    sb.Append($":{@event.Type}");
            }

            if (OtherFamilyMember != null)
            {
                sb.Append($" {OtherFamilyMember.Name}");
            }

            return sb.ToString();
        }

        private GedcomDateRecord GetIndividualEventDate()
        {
            return IndividualEvent.Date;
        }

        private GedcomDateRecord GetFamilyEventDate()
        {
            return FamilyEvent.Date;
        }
    }
}