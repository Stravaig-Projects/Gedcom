using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomFamilyLinkRecord
    {
        public static readonly GedcomTag ChildToFamilyTag = "FAMC".AsGedcomTag();
        public static readonly GedcomTag SpouseToFamilyTag = "FAMS".AsGedcomTag();
        public static readonly GedcomTag[] FamilyTags =
        {
            ChildToFamilyTag, 
            SpouseToFamilyTag,
        };
        
        private readonly GedcomRecord _record;
        private readonly GedcomDatabase _database;

        public GedcomFamilyLinkRecord(GedcomRecord record, GedcomDatabase database)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            _database = database;
            _record = record;
            if (!FamilyTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known family type. One of {string.Join(", ", FamilyTags.Select(ft=>ft.ToString()))}.");
        }

        public GedcomFamilyType Type =>
            _record.Tag == ChildToFamilyTag
                ? GedcomFamilyType.ChildToFamily
                : GedcomFamilyType.SpouseToFamily;
        
        public GedcomPointer Link => new GedcomPointer(_record.Value);
        
        public GedcomFamilyRecord Family => _database.FamilyRecords[Link];
    }
}