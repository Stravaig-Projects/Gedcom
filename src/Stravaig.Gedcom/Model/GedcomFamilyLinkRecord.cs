using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly Lazy<Pedigree> _lazyPedigree;

        public GedcomFamilyLinkRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (!FamilyTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known family type. One of {string.Join(", ", FamilyTags.Select(ft=>ft.ToString()))}.");
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazyPedigree = new Lazy<Pedigree>(GetPedigree);
        }

        private Pedigree GetPedigree()
        {
            var pedigreeRecord = _record.Children.FirstOrDefault(r => r.Tag == PedigreeTag);
            switch (pedigreeRecord?.Value.ToLowerInvariant())
            {
                case "adopted":
                    return Pedigree.Adopted;
                case "birth":
                    return Pedigree.Biological;
                case "foster":
                    return Pedigree.Fostered;
                case "sealing":
                    return Pedigree.Sealed;
                case "step": // Non-standard: Used by MobileFamilyTree.
                    return Pedigree.Step;
                default:
                    return Pedigree.Biological;
            }
        }

        public GedcomFamilyType Type =>
            _record.Tag == ChildToFamilyTag
                ? GedcomFamilyType.ChildToFamily
                : GedcomFamilyType.SpouseToFamily;
        
        public GedcomPointer Link => new GedcomPointer(_record.Value);
        
        public GedcomFamilyRecord Family => _database.FamilyRecords[Link];

        public Pedigree Pedigree => _lazyPedigree.Value;

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
    }
}