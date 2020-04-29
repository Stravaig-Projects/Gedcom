using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class IndividualTimelineExtensions
    {
        public static TimelineEntry[] GetTimeline(this GedcomIndividualRecord subject, bool includeNonSubjectEventsBeyondSubjectLife)
        {
            var events = GetAllEvents(subject);
#if DEBUG
            events = events.ToArray();
#endif

            // If the timeline entry has no date then it can't be positioned in
            // the timeline.
            events = events.Where(te => te.Date != null);
#if DEBUG
            events = events.ToArray();
#endif
            
            
            if (!includeNonSubjectEventsBeyondSubjectLife)
            {
                events = events.Where(te => IsSubjectEntryOrIsInSubjectLifetime(subject, te));
            }
#if DEBUG
            events = events.ToArray();
#endif

            events = events.OrderBy(te => te.Date);
            var result = events.ToArray();
            return result;
        }

        private static bool IsSubjectEntryOrIsInSubjectLifetime(GedcomIndividualRecord subject, TimelineEntry te)
        {
            if (te.Type == TimelineEntry.EventType.SubjectEvent)
                return true;
            if (te.Type == TimelineEntry.EventType.SubjectAttribute)
                return true;

            var birthDate = subject.BirthEvent?.Date;
            var deathDate = subject.DeathEvent?.Date;
            return te.Date.IsBetween(birthDate, deathDate);
        }

        private static IEnumerable<TimelineEntry> GetBirthAndDeathOnly(GedcomIndividualRecord subject)
        {
            if (subject.BirthEvent != null)
                yield return new TimelineEntry(subject, subject.BirthEvent);
            if (subject.DeathEvent != null)
                yield return new TimelineEntry(subject, subject.DeathEvent);
        }

        private static IEnumerable<TimelineEntry> GetAllEvents(GedcomIndividualRecord subject)
        {
            HashSet<GedcomPointer> individualsYielded = new HashSet<GedcomPointer>();
            
            // The subject's own events
            foreach (var individualEvent in subject.Events)
            {
                var result = new TimelineEntry(subject, individualEvent);
                yield return result;
            }

            // The subject's own attributes
            foreach (var individualAttribute in subject.Attributes)
            {
                var result = new TimelineEntry(subject, individualAttribute);
                yield return result;
            }

            individualsYielded.Add(subject.CrossReferenceId);

            // Events for which the subject is a child of the family.
            foreach (var family in subject.ChildToFamilies)
            {
                foreach (var familyEvent in family.Events)
                {
                    var result = new TimelineEntry(subject, family, familyEvent);
                    yield return result;
                }

                foreach (var parent in family.Spouses)
                {
                    if (!individualsYielded.Contains(parent.CrossReferenceId))
                    {
                        foreach (var parentEvent in parent.Events.Where(e => e.IsInterestingParentEvent()))
                        {
                            var result = new TimelineEntry(subject, parent, parentEvent);
                            yield return result;
                        }
                        individualsYielded.Add(parent.CrossReferenceId);
                    }
                }
                
                foreach (var sibling in family.Children)
                {
                    if (!individualsYielded.Contains(sibling.CrossReferenceId))
                    {
                        foreach (var siblingEvent in sibling.Events.Where(e => e.IsInterestingSiblingEvent()))
                        {
                            var result = new TimelineEntry(subject, sibling, siblingEvent);
                            yield return result;
                        }
                        individualsYielded.Add(sibling.CrossReferenceId);
                    }
                }
            }
            
            // Events for which the subject is a spouse
            foreach (var family in subject.SpouseToFamilies)
            {
                foreach (var familyEvent in family.Events)
                {
                    var result = new TimelineEntry(subject, family, familyEvent);
                    yield return result;
                }

                foreach (var spouse in family.Spouses)
                {
                    if (!individualsYielded.Contains(spouse.CrossReferenceId))
                    {
                        foreach (var individualEvent in spouse.Events.Where(e => e.IsInterestingSpouseEvent()))
                        {
                            var result = new TimelineEntry(subject, spouse, individualEvent);
                            yield return result;
                        }
                        individualsYielded.Add(spouse.CrossReferenceId);
                    }
                }
                
                foreach (var child in family.Children)
                {
                    if (!individualsYielded.Contains(child.CrossReferenceId))
                    {
                        foreach (var individualEvent in child.Events.Where(e => e.IsInterestingChildEvent()))
                        {
                            var result = new TimelineEntry(subject, child, individualEvent);
                            yield return result;
                        }
                        individualsYielded.Add(child.CrossReferenceId);
                    }
                }
            }

        }

        private static readonly GedcomTag[] InterestingSpouseEvents =
        {
            GedcomIndividualEventRecord.DeathTag,
            GedcomIndividualEventRecord.BuriedTag,
            GedcomIndividualEventRecord.CrematedTag,
        };
        private static bool IsInterestingSpouseEvent(this EventRecord @event)
        {
            return InterestingSpouseEvents.Contains(@event.Tag);
        }

        private static readonly GedcomTag[] InterestingParentEvents =
        {
            GedcomIndividualEventRecord.DeathTag,
            GedcomIndividualEventRecord.BuriedTag,
            GedcomIndividualEventRecord.CrematedTag,
        };

        private static bool IsInterestingParentEvent(this EventRecord @event)
        {
            return InterestingParentEvents.Contains(@event.Tag);
        }

        private static readonly GedcomTag[] InterestingSiblingEvents =
        {
            GedcomIndividualEventRecord.BirthTag,
            GedcomIndividualEventRecord.DeathTag,
            GedcomIndividualEventRecord.BuriedTag,
            GedcomIndividualEventRecord.CrematedTag,
        };

        private static bool IsInterestingSiblingEvent(this EventRecord @event)
        {
            return InterestingSiblingEvents.Contains(@event.Tag);
        }

        private static readonly GedcomTag[] InterestingChildEvents =
        {
            GedcomIndividualEventRecord.BirthTag,
            GedcomIndividualEventRecord.AdoptionTag,
            GedcomIndividualEventRecord.DeathTag,
            GedcomIndividualEventRecord.BuriedTag,
            GedcomIndividualEventRecord.CrematedTag,
        };

        private static bool IsInterestingChildEvent(this EventRecord @event)
        {
            return InterestingChildEvents.Contains(@event.Tag);
        }
    }
}