using System;
using System.Linq;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class IndividualAlivenessExtensions
    {
        public static bool IsDead(this GedcomIndividualRecord subject)
        {
            DateTime now = DateTime.UtcNow;
            
            var deathEvent = subject.DeathEvent;
            var deathDateRecord = deathEvent?.Date;
            DateTime? deathDate = deathDateRecord?.EndOfExtent;
            if (deathDate.HasValue)
            {
                bool result = deathDate.Value < now;
                return result;
            }
            
            var birthEvent = subject.BirthEvent;
            var birthDateRecord = birthEvent?.Date;
            DateTime? birthDate = birthDateRecord?.EndOfExtent;
            if (birthDate.HasValue)
            {
                try
                {
                    deathDate = birthDate.Value.AddYears(subject.AssumedDeathAge);
                    bool result = deathDate.Value < now;
                    return result;
                }
                catch (Exception ex)
                {
                    throw new GedcomDataException(
                        $"The subject's ({subject.CrossReferenceId}: {subject.NameWithoutMarker}) assumed death date (\"{birthEvent.Date.RawDateValue}\"={birthDate.Value} + {subject.AssumedDeathAge} years) cannot be calculated.",
                        ex);
                }
            }

            return subject.Children()
                .Select(c => c.Relative)
                .Where(c => c.IsBirthDateKnown())
                .Select(c => c.BirthEvent.Date?.EndOfExtent)
                .Where(d => d.HasValue)
                .Select(d => d.Value.AddYears(subject.AssumedDeathAge))
                .Any(d => d < now);
        }
        
        public static bool IsAlive(this GedcomIndividualRecord subject)
        {
            return !IsDead(subject);
        }

        public static bool IsBirthDateKnown(this GedcomIndividualRecord subject)
        {
            return subject.BirthEvent?.Date?.HasCoherentDate ?? false;
        }
        
        public static bool IsDeathDateKnown(this GedcomIndividualRecord subject)
        {
            return subject.DeathEvent?.Date?.HasCoherentDate ?? false;
        }

        public static bool IsLifespanKnown(this GedcomIndividualRecord subject)
        {
            return subject.IsBirthDateKnown() && subject.IsDeathDateKnown();
        }

    }
}