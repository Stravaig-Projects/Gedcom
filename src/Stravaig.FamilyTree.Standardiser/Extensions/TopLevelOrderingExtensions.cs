using System;
using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser.Extensions
{
    public static class TopLevelOrderingExtensions
    {
        private static readonly GedcomTag ChangeTag = "CHAN".AsGedcomTag();
        private static readonly GedcomTag MultimediaTag = "OBJE".AsGedcomTag();

        public static IEnumerable<GedcomRecord> OrderChildren(this GedcomDatabase database, GedcomRecord topLevelRecord)
        {
            var parentTag = topLevelRecord.Tag;
            var pointer = topLevelRecord.CrossReferenceId;
            if (parentTag == GedcomIndividualRecord.Tag && pointer.HasValue)
            {
                var person = database.IndividualRecords[pointer.Value];
                return OrderRecordsForPerson(person);
            }
            if (parentTag == GedcomFamilyRecord.FamilyTag && pointer.HasValue)
            {
                var family = database.FamilyRecords[pointer.Value];
                return OrderRecordsForFamily(family);
            }

            if (parentTag == GedcomSourceRecord.SourceTag && pointer.HasValue)
            {
                var source = database.SourceRecords[pointer.Value];
                return OrderRecordsForSource(source);
            }
            return topLevelRecord.Children;
        }

        private static IEnumerable<GedcomRecord> OrderRecordsForSource(GedcomSourceRecord source)
        {
            var records = source.UnderlyingRecord.Children.ToList();
            var tagSequence = new GedcomTag[]
            {
                GedcomTitleRecord.TitleTag,
                GedcomDateRecord.DateTag,
                GedcomSourcePublicationFactsRecord.PublicationTag,
                GedcomSourceRecord.SourceOriginatorTag, // Author
                GedcomSourceRecord.AgencyTag,
                GedcomSourceRecord.FiledByEntryTag,
                GedcomUserReferenceNumberTypeRecord.ReferenceTypeTag,
                GedcomUserReferenceNumberRecord.ReferenceTag,
                GedcomPlaceRecord.PlaceTag,
                GedcomTextRecord.TextTag,
                GedcomNoteRecord.NoteTag,
                GedcomSourceRecord.ChangeTag,
                GedcomSourceRecord.ObjectTag,
                GedcomLabelRecord.LabelTag,
            };

            foreach (var tag in tagSequence)
            {
                var taggedRecords = records.Where(r => r.Tag == tag).ToList();
                foreach (var record in taggedRecords)
                {
                    yield return record;
                    records.Remove(record);
                }
            }

            foreach (var record in records.OrderBy(r => r.Tag))
            {
                Console.WriteLine(record);
                yield return record;
            }
        }

        private static IEnumerable<GedcomRecord> OrderRecordsForFamily(GedcomFamilyRecord family)
        {
            var records = family.UnderlyingRecord.Children;
            var restrictionNotice = records.FirstOrDefault(r => r.Tag == GedcomIndividualRecord.RestrictionNoticeTag);
            if (restrictionNotice != null)
                yield return restrictionNotice;
            
            var events = family.Events
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Tag);
            foreach (var @event in events)
                yield return @event.UnderlyingRecord;

            var husband = records.FirstOrDefault(r => r.Tag == GedcomFamilyRecord.HusbandTag);
            if (husband != null)
                yield return husband;

            var wife = records.FirstOrDefault(r => r.Tag == GedcomFamilyRecord.WifeTag);
            if (wife != null)
                yield return wife;

            var children = records.Where(r => r.Tag == GedcomFamilyRecord.ChildTag);
            foreach (var child in children)
                yield return child;
            
            // TODO: Number of Children
            
            // TODO: LDS Submission
            
            // TODO: LDS Spouse sealing
            
            // TODO: REFN
            
            // TODO: RIN
            
            var changeDate = records.FirstOrDefault(r => r.Tag == ChangeTag);
            if (changeDate != null)
                yield return changeDate;

            var notes = records.Where(r => r.Tag == GedcomNoteRecord.NoteTag)
                .OrderBy(r => r.Value);
            foreach (var note in notes)
                yield return note;
            
            var sources = records.Where(r => r.Tag == GedcomSourceRecord.SourceTag)
                .OrderBy(r => r.Value);
            foreach (var source in sources)
                yield return source;

            var multimedia = records.Where(r => r.Tag == MultimediaTag)
                .OrderBy(r => r.Value);
            foreach (var obj in multimedia)
                yield return obj;
        }

        private static IEnumerable<GedcomRecord> OrderRecordsForPerson(GedcomIndividualRecord person)
        {
            var records = person.UnderlyingRecord.Children;
            var restrictionNotice = records.FirstOrDefault(r => r.Tag == GedcomIndividualRecord.RestrictionNoticeTag);
            if (restrictionNotice != null)
                yield return restrictionNotice;

            var names = person.Names
                .OrderBy(n => n.Type)
                .ThenBy(n => n.Name)
                .Select(n => n.UnderlyingRecord);
            foreach (var name in names)
                yield return name;

            var sex = records.FirstOrDefault(r => r.Tag == GedcomIndividualRecord.SexTag);
            if (sex != null)
                yield return sex;

            var events = person.Events
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Tag);
            foreach (var @event in events)
                yield return @event.UnderlyingRecord;

            var attributes = person.Attributes
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Tag);
            foreach (var attr in attributes)
                yield return attr.UnderlyingRecord;
            
            // TODO: LDS Ordinance

            var childToFamilies = person.FamilyLinks
                .Where(fl => fl.Type == GedcomFamilyType.ChildToFamily)
                .OrderBy(fl => fl.Link);
            foreach (var childToFamily in childToFamilies)
                yield return childToFamily.UnderlyingRecord;

            var spouseToFamilies = person.FamilyLinks
                .Where(fl => fl.Type == GedcomFamilyType.SpouseToFamily)
                .OrderBy(fl => fl.Link);
            foreach (var spouseToFamily in spouseToFamilies)
                yield return spouseToFamily.UnderlyingRecord;
            
            // TODO: SUBM
            
            // TODO: Association
            
            // TODO: ALIA
            
            // TODO: ANCI
            
            // TODO: DESI
            
            // TODO: RFN
            
            // TODO: AFN
            
            // TODO: REFN
            
            // TODO: RIN
            
            // TODO: Change Date

            var changeDate = records.FirstOrDefault(r => r.Tag == ChangeTag);
            if (changeDate != null)
                yield return changeDate;

            var notes = records.Where(r => r.Tag == GedcomNoteRecord.NoteTag)
                .OrderBy(r => r.Value);
            foreach (var note in notes)
                yield return note;
            
            var sources = records.Where(r => r.Tag == GedcomSourceRecord.SourceTag)
                .OrderBy(r => r.Value);
            foreach (var source in sources)
                yield return source;

            var multimedia = records.Where(r => r.Tag == MultimediaTag)
                .OrderBy(r => r.Value);
            foreach (var obj in multimedia)
                yield return obj;
        }
    }
}