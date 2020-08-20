using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser.Extensions
{
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