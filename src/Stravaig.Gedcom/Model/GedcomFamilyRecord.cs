using System;
using System.Linq;
using System.Net.NetworkInformation;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomFamilyRecord
    {
        private readonly GedcomRecord _record;
        private readonly GedcomDatabase _database;
        public static readonly GedcomTag FamilyTag = "FAM".AsGedcomTag();
        public static readonly GedcomTag HusbandTag = "HUSB".AsGedcomTag();
        public static readonly GedcomTag WifeTag = "WIFE".AsGedcomTag();
        public static readonly GedcomTag ChildTag = "CHIL".AsGedcomTag();

        public static readonly GedcomTag[] SpouseTags =
        {
            HusbandTag,
            WifeTag,
        };

        public GedcomFamilyRecord(GedcomRecord record, GedcomDatabase database)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            _database = database;
            
            if (record.Tag != FamilyTag)
                throw new ArgumentException($"Must be a \"FAM_RECORD\" ({FamilyTag}).");
            
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException($"A \"FAM_RECORD\" ({FamilyTag}) must have a CrossReferenceId.");
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
    }
}