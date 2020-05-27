using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomFamilyLinkRecord : Record
    {
        public static readonly GedcomTag ChildToFamilyTag = "FAMC".AsGedcomTag();
        public static readonly GedcomTag SpouseToFamilyTag = "FAMS".AsGedcomTag();
        private static readonly GedcomTag PedigreeTag = "PEDI".AsGedcomTag();
        public static readonly GedcomTag[] FamilyTags =
        {
            ChildToFamilyTag, 
            SpouseToFamilyTag,
        };

        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;
        private readonly Lazy<Qualification> _lazyQualification;

        public GedcomFamilyLinkRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (!FamilyTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known family type. One of {string.Join(", ", FamilyTags.Select(ft=>ft.ToString()))}.");
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazyQualification = new Lazy<Qualification>(GetQualification);
        }

        private Qualification GetQualification()
        {
            var pedigreeRecord = _record.Children.FirstOrDefault(r => r.Tag == PedigreeTag);
            switch (pedigreeRecord?.Value.ToLowerInvariant())
            {
                case "adopted":
                    return Qualification.Adopted;
                case "birth":
                    return Qualification.Biological;
                case "foster":
                    return Qualification.Fostered;
                case "sealing":
                    return Qualification.Sealed;
                case "step": // Non-standard: Used by MobileFamilyTree.
                    return Qualification.Step;
                default:
                    return Qualification.Biological;
            }
        }

        public GedcomFamilyType Type =>
            _record.Tag == ChildToFamilyTag
                ? GedcomFamilyType.ChildToFamily
                : GedcomFamilyType.SpouseToFamily;
        
        public GedcomPointer Link => new GedcomPointer(_record.Value);
        
        public GedcomFamilyRecord Family => _database.FamilyRecords[Link];

        public Qualification Qualification => _lazyQualification.Value;

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
    }
}