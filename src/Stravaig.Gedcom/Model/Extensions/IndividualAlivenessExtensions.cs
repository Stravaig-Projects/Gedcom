using System;

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
                deathDate = birthDate.Value.AddYears(subject.AssumedDeathAge);
                bool result = deathDate.Value < now;
                return result;
            }

            return false;
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