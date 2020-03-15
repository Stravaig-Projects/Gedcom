using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public class GedcomIndividualEventRecord
    {
        public static readonly GedcomTag AgeTag = "AGE".AsGedcomTag();
        
        public static readonly GedcomTag BirthTag = "BIRT".AsGedcomTag();
        public static readonly GedcomTag DeathTag = "DEAT".AsGedcomTag();

        public static readonly GedcomTag[] EventTags = new[]
        {
            BirthTag,
            DeathTag
        };
        
        private readonly GedcomRecord _record;

        public GedcomIndividualEventRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            if (!EventTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known event type. One of {string.Join(", ", EventTags.Select(et=>et.ToString()))}.");
        }

        public GedcomTag Tag => _record.Tag;

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

        public GedcomDateRecord Date
        {
            get
            {
                var record = _record.Children.FirstOrDefault(r => r.Tag == GedcomDateRecord.DateTag);
                if (record != null)
                    return new GedcomDateRecord(record);
                return null;
            }
        } 
    }
}