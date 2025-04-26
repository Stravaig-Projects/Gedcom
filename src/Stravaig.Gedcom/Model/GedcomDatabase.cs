using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stravaig.Gedcom.Settings;

namespace Stravaig.Gedcom.Model
{
    public class GedcomDatabase
    {
        private readonly List<GedcomRecord> _records = new List<GedcomRecord>();
        //private readonly Dictionary<GedcomPointer, GedcomRecord> _crossReferencedRecords = new Dictionary<GedcomPointer, GedcomRecord>();
        private readonly Dictionary<GedcomPointer, GedcomIndividualRecord> _individualRecords = new Dictionary<GedcomPointer, GedcomIndividualRecord>();
        private readonly Dictionary<GedcomPointer, GedcomFamilyRecord> _familyRecords = new Dictionary<GedcomPointer, GedcomFamilyRecord>();
        private readonly Dictionary<GedcomPointer, GedcomNoteRecord> _noteRecords = new Dictionary<GedcomPointer, GedcomNoteRecord>();
        private readonly Dictionary<GedcomPointer, GedcomSourceRecord> _sourceRecords = new Dictionary<GedcomPointer, GedcomSourceRecord>();
        private readonly Dictionary<GedcomPointer, GedcomLabelRecord> _labelRecords = new Dictionary<GedcomPointer, GedcomLabelRecord>();
        private readonly Dictionary<GedcomPointer, GedcomObjectRecord> _objectRecords = new Dictionary<GedcomPointer, GedcomObjectRecord>();

        public DatabaseSettings Settings { get; } = new DatabaseSettings();

        public void Populate(GedcomRecordReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            GedcomRecord record;
            while ((record = reader.ReadRecord()) != null)
            {
                ProcessRecord(record);
            }
        }

        public async Task PopulateAsync(GedcomRecordReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            GedcomRecord record;
            while ((record = await reader.ReadRecordAsync()) != null)
            {
                ProcessRecord(record);
            }
        }

        public void AddRecord(GedcomRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            ProcessRecord(record);
        }

        public IReadOnlyList<GedcomRecord> Records => _records;

        //public IReadOnlyDictionary<GedcomPointer, GedcomRecord> CrossReferencedRecords => _crossReferencedRecords;
        public IReadOnlyDictionary<GedcomPointer, GedcomIndividualRecord> IndividualRecords => _individualRecords;
        public IReadOnlyDictionary<GedcomPointer, GedcomFamilyRecord> FamilyRecords => _familyRecords;
        public IReadOnlyDictionary<GedcomPointer, GedcomNoteRecord> NoteRecords => _noteRecords;
        public IReadOnlyDictionary<GedcomPointer, GedcomSourceRecord> SourceRecords => _sourceRecords;
        public IReadOnlyDictionary<GedcomPointer, GedcomLabelRecord> LabelRecords => _labelRecords;

        public IReadOnlyDictionary<GedcomPointer, GedcomObjectRecord> ObjectRecords => _objectRecords;

        private void ProcessRecord(GedcomRecord record)
        {
            _records.Add(record);
            if (record.CrossReferenceId.HasValue)
            {
                var pointer = record.CrossReferenceId.Value;
                //_crossReferencedRecords.Add(pointer, record);
                if (record.Tag == GedcomIndividualRecord.Tag)
                    _individualRecords.Add(pointer, new GedcomIndividualRecord(record, this));
                else if (record.Tag == GedcomFamilyRecord.FamilyTag)
                    _familyRecords.Add(pointer, new GedcomFamilyRecord(record, this));
                else if (record.Tag == GedcomNoteRecord.NoteTag)
                    _noteRecords.Add(pointer, new GedcomNoteRecord(record, this));
                else if (record.Tag == GedcomSourceRecord.SourceTag)
                    _sourceRecords.Add(pointer, new GedcomSourceRecord(record, this));
                else if (record.Tag == GedcomLabelRecord.LabelTag)
                    _labelRecords.Add(pointer, new GedcomLabelRecord(record, this));
                else if (record.Tag == GedcomObjectRecord.ObjectTag)
                    _objectRecords.Add(pointer, new GedcomObjectRecord(record, this));
            }
        }
    }
}
