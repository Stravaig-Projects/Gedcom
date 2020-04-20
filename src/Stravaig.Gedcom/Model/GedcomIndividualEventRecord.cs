using System;
using System.Diagnostics;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    [DebuggerDisplay("{Tag}:{Type}")]
    public class GedcomIndividualEventRecord : EventRecord
    {
        public static readonly GedcomTag AgeTag = "AGE".AsGedcomTag();
        
        public static readonly GedcomTag BirthTag = "BIRT".AsGedcomTag();
        public static readonly GedcomTag DeathTag = "DEAT".AsGedcomTag();

        public static readonly GedcomTag[] EventTags =
        {
            BirthTag,
            DeathTag,
        };
        
        public GedcomIndividualEventRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (!EventTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known event type. One of {string.Join(", ", EventTags.Select(et=>et.ToString()))}.");
        }


        public int? Age
        {
            get
            {
                var ageRecord = _record.Children.FirstOrDefault(r => r.Tag == AgeTag);
                if (ageRecord == null || string.IsNullOrWhiteSpace(ageRecord.Value))
                    return null;
                if (int.TryParse(ageRecord.Value, out var result))
                    return result;
                return null;
            }
        }

    }
}