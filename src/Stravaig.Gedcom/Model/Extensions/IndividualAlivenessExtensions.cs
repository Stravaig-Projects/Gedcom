using System;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class IndividualAlivenessExtensions
    {
        public static bool IsDead(this GedcomIndividualRecord individualRecord)
        {
            DateTime now = DateTime.UtcNow;
            
            var deathEvent = individualRecord.DeathEvent;
            var deathDateRecord = deathEvent?.Date;
            DateTime? deathDate = deathDateRecord?.EndOfExtent;
            if (deathDate.HasValue)
            {
                bool result = deathDate.Value < now;
                return result;
            }
            
            var birthEvent = individualRecord.BirthEvent;
            var birthDateRecord = birthEvent?.Date;
            DateTime? birthDate = birthDateRecord?.EndOfExtent;
            if (birthDate.HasValue)
            {
                deathDate = birthDate.Value.AddYears(individualRecord.AssumedDeathAge);
                bool result = deathDate.Value < now;
                return result;
            }

            return false;
        }
        
        public static bool IsAlive(this GedcomIndividualRecord individualRecord)
        {
            return !IsDead(individualRecord);
        }
    }
}