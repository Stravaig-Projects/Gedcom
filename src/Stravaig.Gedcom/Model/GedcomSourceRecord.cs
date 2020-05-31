using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Stravaig.Gedcom.Extensions;

// From Pages 27. 28
//
// SOURCE_RECORD:=
// n @<XREF:SOUR>@ SOUR {1:1}
//   +1 DATA {0:1}
//     +2 EVEN <EVENTS_RECORDED> {0:M} p.50
//       +3 DATE <DATE_PERIOD> {0:1} p.46
//       +3 PLAC <SOURCE_JURISDICTION_PLACE> {0:1} p.62
//     +2 AGNC <RESPONSIBLE_AGENCY> {0:1} p.60
//     +2 <<NOTE_STRUCTURE>> {0:M} p.37
//   +1 AUTH <SOURCE_ORIGINATOR> {0:1} p.62
//     +2 [CONC|CONT] <SOURCE_ORIGINATOR> {0:M} p.62
//   +1 TITL <SOURCE_DESCRIPTIVE_TITLE> {0:1} p.62
//     +2 [CONC|CONT] <SOURCE_DESCRIPTIVE_TITLE> {0:M} p.62
//   +1 ABBR <SOURCE_FILED_BY_ENTRY> {0:1} p.62
//   +1 PUBL <SOURCE_PUBLICATION_FACTS> {0:1} p.62
//     +2 [CONC|CONT] <SOURCE_PUBLICATION_FACTS> {0:M} p.62
//   +1 TEXT <TEXT_FROM_SOURCE> {0:1} p.63
//     +2 [CONC|CONT] <TEXT_FROM_SOURCE> {0:M} p.63
//   +1 <<SOURCE_REPOSITORY_CITATION>> {0:M} p.40
//   +1 REFN <USER_REFERENCE_NUMBER> {0:M} p.63, 64
//     +2 TYPE <USER_REFERENCE_TYPE> {0:1} p.64
//   +1 RIN <AUTOMATED_RECORD_ID> {0:1} p.43
//   +1 <<CHANGE_DATE>> {0:1} p.31
//   +1 <<NOTE_STRUCTURE>> {0:M} p.37
//   +1 <<MULTIMEDIA_LINK>> {0:M} p.37, 26
//
// EVENTS_RECORDED:= {Size=1:90}
// [
//   <EVENT_ATTRIBUTE_TYPE>
//   |
//   <EVENTS_RECORDED>, <EVENT_ATTRIBUTE_TYPE>
// ]
//   An enumeration of the different kinds of events that were recorded in a
//   particular source. Each enumeration is separated by a comma. Such as a
//   parish register of births, deaths, and marriages would be BIRT, DEAT,
//   MARR.
//
// SOURCE_JURISDICTION_PLACE:= {Size=1:120}
//   <PLACE_NAME>
//   The name of the lowest jurisdiction that encompasses all lower-level
//   places named in this source.  For example, "Oneida, Idaho" would be used
//   as a source jurisdiction place for events occurring in the various towns
//   within Oneida County. "Idaho" would be the source jurisdiction place if
//   the events recorded took place in other counties as well as Oneida County.
//
// RESPONSIBLE_AGENCY:= {Size=1:120}
//   The organization, institution, corporation, person, or other entity that
//   has responsibility for the associated context. For example, an employer
//   of a person of an associated occupation, or a church that administered
//   rites or events, or an organization responsible for creating and/or
//   archiving records.
//
// SOURCE_ORIGINATOR:= {Size=1:248}
//   The person, agency, or entity who created the record. For a published
//   work, this could be the author, compiler, transcriber, abstractor, or
//   editor. For an unpublished source, this may be an individual, a
//   government agency, church organization, or private organization, etc.
//
// SOURCE_DESCRIPTIVE_TITLE:= {Size=1:248}
//   The title of the work, record, or item and, when appropriate, the title of
//   the larger work or series of which it is a part.
//   For a published work, a book for example, might have a title plus the
//   title of the series of which the book is a part. A magazine article would
//   have a title plus the title of the magazine that published the article
//   For An unpublished work, such as:
//     ! A letter might include the date, the sender, and the receiver.
//     ! A transaction between a buyer and seller might have their names and
//       the transaction date.
//     ! A family Bible containing genealogical information might have past
//       and present owners and a physical description of the book.
//     ! A personal interview would cite the informant and interviewer.
//
// SOURCE_FILED_BY_ENTRY:= {Size= 1:60}
//   This entry is to provide a short title used for sorting, filing, and
//   retrieving source records.
//
// SOURCE_PUBLICATION_FACTS:= {Size=1:248}
//   When and where the record was created. For published works, this includes
//   information such as the city of publication, name of the publisher, and
//   year of publication.
//   For an unpublished work, it includes the date the record was created and
//   the place where it was created. For example, the county and state of
//   residence of a person making a declaration for a pension or the city and
//   state of residence of the writer of a letter.
//
// USER_REFERENCE_NUMBER:= {Size=1:20}
//   A user-defined number or text that the submitter uses to identify this
//   record. For instance, it may be a record number within the submitter's
//   automated or manual system, or it may be a page and position number on a
//   pedigree chart.

namespace Stravaig.Gedcom.Model
{
    public class GedcomSourceRecord : Record
    {
        public static readonly GedcomTag SourceTag = "SOUR".AsGedcomTag();
        private static readonly GedcomTag DataTag = "DATA".AsGedcomTag();
        private static readonly GedcomTag AgencyTag = "AGNC".AsGedcomTag();
        private static readonly GedcomTag SourceOriginatorTag = "AUTH".AsGedcomTag();
        private static readonly GedcomTag FiledByEntryTag = "ABBR".AsGedcomTag();

        private readonly Lazy<GedcomTitleRecord> _lazyTitle;
        private readonly Lazy<GedcomTextRecord> _lazyText;
        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;
        private readonly Lazy<string> _lazyResponsibleAgency;
        private readonly Lazy<string> _lazyOriginator;
        private readonly Lazy<GedcomSourcePublicationFactsRecord> _lazyPublication;
        private readonly Lazy<string> _lazyFiledByEntry;
        private readonly Lazy<GedcomDateRecord> _lazyDate;
        private readonly Lazy<GedcomUserReferenceNumberRecord[]> _lazyReferences;
        private readonly Lazy<Record[]> _lazyReferencedRecords;
        private readonly Lazy<GedcomLabelRecord[]> _lazyLabels;
        private readonly Lazy<string[]> _lazyLabelTitles;
        
        public GedcomSourceRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != SourceTag)
                throw new ArgumentException($"Expected a \"SOUR\" record, but got a \"{record.Tag}\" instead.");
            
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException("Expected the record to have a cross reference id, but it did not.", nameof(record));
            
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazyTitle = new Lazy<GedcomTitleRecord>(GetTitleRecord);
            _lazyText = new Lazy<GedcomTextRecord>(GetTextRecord);
            _lazyResponsibleAgency = new Lazy<string>(GetResponsibleAgency);
            _lazyOriginator = new Lazy<string>(GetSourceOriginator);
            _lazyPublication = new Lazy<GedcomSourcePublicationFactsRecord>(GetPublicationFactsRecord);
            _lazyFiledByEntry = new Lazy<string>(GetFiledByEntry);
            _lazyDate = new Lazy<GedcomDateRecord>(GetDateRecord);
            _lazyReferences = new Lazy<GedcomUserReferenceNumberRecord[]>(GetReferences);
            _lazyReferencedRecords = new Lazy<Record[]>(GetReferencedRecords);
            _lazyLabels = new Lazy<GedcomLabelRecord[]>(GetLabels);
            _lazyLabelTitles = new Lazy<string[]>(() => Labels.Select(l => l.Title).ToArray());
        }

        private GedcomUserReferenceNumberRecord[] GetReferences()
        {
            var result = _record.Children.Where(r => r.Tag == GedcomUserReferenceNumberRecord.ReferenceTag)
                .Select(r => new GedcomUserReferenceNumberRecord(r, _database))
                .ToArray();
            return result;
        }

        private GedcomSourcePublicationFactsRecord GetPublicationFactsRecord()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == GedcomSourcePublicationFactsRecord.PublicationTag);
            if (record != null)
                return new GedcomSourcePublicationFactsRecord(record, _database);
            return null;
        }
        
        private GedcomTitleRecord GetTitleRecord()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == GedcomTitleRecord.TitleTag);
            if (record != null)
                return new GedcomTitleRecord(record, _database);
            return null;
        }

        private GedcomTextRecord GetTextRecord()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == GedcomTextRecord.TextTag);
            if (record != null)
                return new GedcomTextRecord(record, _database);
            return null;
        }

        private string GetFiledByEntry()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == FiledByEntryTag);
            return record?.Value;
        }

        public Record[] GetReferencedRecords()
        {
            return GetReferencedByRecordsImpl().ToArray();
        }

        private IEnumerable<Record> GetReferencedByRecordsImpl()
        {
            foreach (var individual in _database.IndividualRecords.Values)
            {
                if (individual.Sources.Contains(this))
                    yield return individual;

                foreach (var @event in individual.Events.Where(e => e.Sources.Contains(this)))
                    yield return @event;

                foreach (var attr in individual.Attributes.Where(a => a.Sources.Contains(this)))
                    yield return attr;
            }

            foreach (var family in _database.FamilyRecords.Values)
            {
                if (family.Sources.Contains(this))
                    yield return family;
                foreach (var @event in family.Events.Where(e => e.Sources.Contains(this)))
                    yield return @event;
            }

        }
        
        private string GetResponsibleAgency()
        {
            // The Agency tag being directly under the Source record is
            // non-standard but there is at least one system that produces data
            // in this form.
            var agencyRecord = _record.Children.FirstOrDefault(r => r.Tag == AgencyTag);
            if (agencyRecord == null)
            {
                var dataRecord = _record.Children.FirstOrDefault(r => r.Tag == DataTag);
                if (dataRecord != null)
                    agencyRecord = dataRecord.Children.FirstOrDefault(r => r.Tag == AgencyTag);
            }

            return agencyRecord?.Value;
        }
        
        private string GetSourceOriginator()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == SourceOriginatorTag);
            return record?.Value;
        }

        
        // This is checked in the constructor already.
        // ReSharper disable once PossibleInvalidOperationException
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
        public GedcomDateRecord Date => _lazyDate.Value;

        public string Title => _lazyTitle.Value?.Text;

        public string Text => _lazyText.Value?.Text;

        public string ResponsibleAgency => _lazyResponsibleAgency.Value;
        public string Originator => _lazyOriginator.Value;
        public string PublicationFacts => _lazyPublication.Value?.Text;
        public string FiledByEntry => _lazyFiledByEntry.Value;
        public GedcomUserReferenceNumberRecord[] References => _lazyReferences.Value;

        public Record[] ReferencedBy => _lazyReferencedRecords.Value;
        public GedcomLabelRecord[] Labels => _lazyLabels.Value;
        public string[] LabelTitles => _lazyLabelTitles.Value;
    }
}