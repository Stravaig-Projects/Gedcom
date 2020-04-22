using System;
using System.Collections.Generic;
using System.Linq;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class IndividualTimelineExtensions
    {
        public static TimelineEntry[] GetTimeline(this GedcomIndividualRecord subject, bool extendBeyondLife)
        {
            if (!extendBeyondLife)
            {
                // Don't know when life is, so can't return anything.
                if (subject.BirthEvent?.Date?.BeginningOfExtent == null || 
                    subject.DeathEvent?.Date?.EndOfExtent == null)
                {
                    return GetBirthAndDeathOnly(subject).ToArray();
                }
            }
            
            var events = GetAllEvents(subject);
            if (!extendBeyondLife)
            {
                events = events.Where(te => te.Date != null &&
                                           te.Date.IsBetween(subject.BirthEvent.Date, subject.DeathEvent.Date));
            }

            events = events.OrderBy(te => te.Date);
            
            return events.ToArray();
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
            foreach (var individualEvent in subject.Events)
            {
                var result = new TimelineEntry(subject, individualEvent);
                yield return result;
            }

            foreach (var individualAttribute in subject.Attributes)
            {
                var result = new TimelineEntry(subject, individualAttribute);
                yield return result;
            }

            individualsYielded.Add(subject.CrossReferenceId);

            foreach (var family in subject.ChildToFamilies)
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
                        foreach (var individualEvent in spouse.Events)
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
                        foreach (var individualEvent in child.Events)
                        {
                            var result = new TimelineEntry(subject, child, individualEvent);
                            yield return result;
                        }
                        individualsYielded.Add(child.CrossReferenceId);
                    }
                }
                
            }
            
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
                        foreach (var individualEvent in spouse.Events)
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
                        foreach (var individualEvent in child.Events)
                        {
                            var result = new TimelineEntry(subject, child, individualEvent);
                            yield return result;
                        }
                        individualsYielded.Add(child.CrossReferenceId);
                    }
                }
            }

        }
    }
}