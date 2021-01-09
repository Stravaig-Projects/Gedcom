using System;
using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public abstract class Record
    {
        protected readonly GedcomRecord _record;
        protected readonly GedcomDatabase _database;

        protected Record(GedcomRecord record, GedcomDatabase database)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public GedcomRecord UnderlyingRecord => _record;

        protected GedcomDateRecord GetDateRecord()
        {
            var record = GetChild(GedcomDateRecord.DateTag);
            if (record != null)
                return new GedcomDateRecord(record, _database);
            return null;
        }
        
        protected GedcomNoteRecord[] GetNoteRecords()
        {
            return GetChildren(GedcomNoteRecord.NoteTag)
                .Select(r => r.Value.IsGedcomPointer() 
                    ? _database.NoteRecords[r.Value.AsGedcomPointer()] 
                    : new GedcomNoteRecord(r, _database))
                .ToArray();
        }
        
        protected GedcomSourceRecord[] GetSourceRecords()
        {
            return GetChildren(GedcomSourceRecord.SourceTag)
                .Where(r => r.Value.IsGedcomPointer())
                .Select(r => _database.SourceRecords[r.Value.AsGedcomPointer()])
                .ToArray();
        }

        protected GedcomLabelRecord[] GetLabels()
        {
            return GetChildren(GedcomLabelRecord.LabelTag)
                .Where(r => r.Value.IsGedcomPointer())
                .Select(r => _database.LabelRecords[r.Value.AsGedcomPointer()])
                .ToArray();
        }
        
        protected GedcomRecord GetChild(GedcomTag tag)
        {
            return GetChildren(tag).FirstOrDefault();
        }

        protected T MapChild<T>(GedcomTag tag, Func<GedcomRecord, GedcomDatabase, T> factory) where T : Record
        {
            var record = GetChild(tag);
            if (record != null)
                return factory(record, _database);
            return null;
        }

        protected IEnumerable<GedcomRecord> GetChildren(GedcomTag tag)
        {
            return _record.Children.Where(r => r.Tag == tag);
        }

        protected IEnumerable<GedcomRecord> GetChildren(IEnumerable<GedcomTag> tags)
        {
            return _record.Children.Where(r => tags.Contains(r.Tag));
        }
    }
}