using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomFamilyLinkRecord : Record
    {
        public static readonly GedcomTag ChildToFamilyTag = "FAMC".AsGedcomTag();
        public static readonly GedcomTag SpouseToFamilyTag = "FAMS".AsGedcomTag();
        public static readonly GedcomTag[] FamilyTags =
        {
            ChildToFamilyTag, 
            SpouseToFamilyTag,
        };

        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;

        public GedcomFamilyLinkRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (!FamilyTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known family type. One of {string.Join(", ", FamilyTags.Select(ft=>ft.ToString()))}.");
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
        }

        public GedcomFamilyType Type =>
            _record.Tag == ChildToFamilyTag
                ? GedcomFamilyType.ChildToFamily
                : GedcomFamilyType.SpouseToFamily;
        
        public GedcomPointer Link => new GedcomPointer(_record.Value);
        
        public GedcomFamilyRecord Family => _database.FamilyRecords[Link];

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
    }
}