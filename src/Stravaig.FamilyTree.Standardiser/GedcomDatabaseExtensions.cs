using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser
{
    public static class ChildrenOrderingExtensions
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
            return topLevelRecord.Children;
        }

        private static IEnumerable<GedcomRecord> OrderRecordsForPerson(GedcomIndividualRecord person)
        {
            var records = person.UnderlyingRecord.Children;
            var restrictionNotice = records.FirstOrDefault(r => r.Tag == GedcomIndividualRecord.RestrictionNoticeTag);
            if (restrictionNotice != null)
                yield return restrictionNotice;

            var names = records.Where(r => r.Tag == GedcomNameRecord.NameTag);
            foreach (var name in names)
                yield return name;

            var sex = records.FirstOrDefault(r => r.Tag == GedcomIndividualRecord.SexTag);
            if (sex != null)
                yield return sex;

            var events = person.Events
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Tag); //records.Where(r => GedcomIndividualEventRecord.EventTags.Contains(r.Tag));
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
                .OrderBy(r => r.CrossReferenceId);
            foreach (var note in notes)
                yield return note;
            
            var sources = records.Where(r => r.Tag == GedcomSourceRecord.SourceTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var source in sources)
                yield return source;

            var multimedia = records.Where(r => r.Tag == MultimediaTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var obj in multimedia)
                yield return obj;
        }
    }
    public static class GedcomDatabaseExtensions
    {
        private static readonly GedcomTag HeaderTag = "HEAD".AsGedcomTag();
        private static readonly GedcomTag MultimediaTag = "OBJE".AsGedcomTag();
        private static readonly GedcomTag SubmissionTag = "SUBM".AsGedcomTag();

        public static GedcomRecord[] StandardisedTopLevelRecordOrder(this GedcomDatabase database)
        {
            return database.StandardisedTopLevelRecordOrderImpl().ToArray();
        }
        
        private static IEnumerable<GedcomRecord> StandardisedTopLevelRecordOrderImpl(this GedcomDatabase database)
        {
            var records = database.Records;
            yield return records.First(r => r.Tag == HeaderTag);

            var submissionRecord = records.FirstOrDefault(r => r.Tag == SubmissionTag);
            if (submissionRecord != null)
                yield return submissionRecord;
            
            var orderedIndividualRecords = records
                .Where(r => r.Tag == GedcomIndividualRecord.Tag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var record in orderedIndividualRecords)
                yield return record;

            var orderedFamilyRecords = records
                .Where(r => r.Tag == GedcomFamilyRecord.FamilyTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var record in orderedFamilyRecords)
                yield return record;

            var orderedNoteRecords = records
                .Where(r => r.Tag == GedcomNoteRecord.NoteTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var record in orderedNoteRecords)
                yield return record;
            
            var orderedSourceRecords = records
                .Where(r => r.Tag == GedcomSourceRecord.SourceTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var record in orderedSourceRecords)
                yield return record;
            
            var orderedMultimediaRecords = records
                .Where(r => r.Tag == MultimediaTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var record in orderedMultimediaRecords)
                yield return record;

            var orderedLabelRecords = records
                .Where(r => r.Tag == GedcomLabelRecord.LabelTag)
                .OrderBy(r => r.CrossReferenceId);
            foreach (var record in orderedLabelRecords)
                yield return record;
        }
    }
}