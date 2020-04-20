using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomFamilyRecord : Record
    {
        public static readonly GedcomTag FamilyTag = "FAM".AsGedcomTag();
        public static readonly GedcomTag HusbandTag = "HUSB".AsGedcomTag();
        public static readonly GedcomTag WifeTag = "WIFE".AsGedcomTag();
        public static readonly GedcomTag ChildTag = "CHIL".AsGedcomTag();

        public static readonly GedcomTag[] SpouseTags =
        {
            HusbandTag,
            WifeTag,
        };

        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;
        private readonly Lazy<GedcomFamilyEventRecord[]> _lazyFamilyEvents;
        
        public GedcomFamilyRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != FamilyTag)
                throw new ArgumentException($"Must be a \"FAM_RECORD\" ({FamilyTag}).");
            
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException($"A \"FAM_RECORD\" ({FamilyTag}) must have a CrossReferenceId.");
            
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazyFamilyEvents = new Lazy<GedcomFamilyEventRecord[]>(GetFamilyEvents);
        }

        private GedcomFamilyEventRecord[] GetFamilyEvents()
        {
            return _record.Children
                .Where(r => GedcomFamilyEventRecord.FamilyEventTags.Contains(r.Tag))
                .Select(r => new GedcomFamilyEventRecord(r, _database))
                .ToArray();
        }

        // ReSharper disable once PossibleInvalidOperationException
        // Checked in the ctor.
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public GedcomIndividualRecord[] Spouses => _record.Children
            .Where(r => SpouseTags.Contains(r.Tag))
            .Select(r => _database.IndividualRecords[r.Value.AsGedcomPointer()])
            .ToArray();
        
        public GedcomIndividualRecord[] Children => _record.Children
            .Where(r => r.Tag == ChildTag)
            .Select(r => _database.IndividualRecords[r.Value.AsGedcomPointer()])
            .ToArray();

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
        public GedcomFamilyEventRecord[] Events => _lazyFamilyEvents.Value;
    }
}