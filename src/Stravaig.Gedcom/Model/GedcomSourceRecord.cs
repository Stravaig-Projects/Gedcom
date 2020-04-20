using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomSourceRecord : Record
    {
        public static readonly GedcomTag SourceTag = "SOUR".AsGedcomTag();

        private Lazy<GedcomTitleRecord> _lazyTitle;
        private Lazy<GedcomTextRecord> _lazyText;
        private Lazy<GedcomNoteRecord[]> _lazyNotes;
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

        
        // This is checked in the constructor already.
        // ReSharper disable once PossibleInvalidOperationException
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;

        public string Title => _lazyTitle.Value?.Text;

        public string Text => _lazyText.Value?.Text;
    }
}