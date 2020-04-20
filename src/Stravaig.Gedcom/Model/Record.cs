using System;
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

        protected GedcomDateRecord GetDateRecord()
        {
            var record = _record.Children
                .FirstOrDefault(r => r.Tag == GedcomDateRecord.DateTag);
            if (record != null)
                return new GedcomDateRecord(record, _database);
            return null;
        }
        
        protected GedcomNoteRecord[] GetNoteRecords()
        {
            return _record.Children
                .Where(r => r.Tag == GedcomNoteRecord.NoteTag)
                .Select(r => r.Value.IsGedcomPointer() 
                    ? _database.NoteRecords[r.Value.AsGedcomPointer()] 
                    : new GedcomNoteRecord(r, _database))
                .ToArray();
        }
        
        protected GedcomSourceRecord[] GetSourceRecords()
        {
            return _record.Children
                .Where(r => r.Tag == GedcomSourceRecord.SourceTag)
                .Where(r => r.Value.IsGedcomPointer())
                .Select(r => _database.SourceRecords[r.Value.AsGedcomPointer()])
                .ToArray();
        }
    }
}