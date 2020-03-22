using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom
{
    public class GedcomDatabase
    {
        private readonly List<GedcomRecord> _records;
        private readonly Dictionary<GedcomPointer, GedcomRecord> _crossReferencedRecords;
        private readonly Dictionary<GedcomPointer, GedcomIndividualRecord> _individualRecords;

        public GedcomDatabase()
        {
            _records = new List<GedcomRecord>();
            _crossReferencedRecords = new Dictionary<GedcomPointer, GedcomRecord>();
            _individualRecords = new Dictionary<GedcomPointer, GedcomIndividualRecord>();
        }

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

        public IReadOnlyDictionary<GedcomPointer, GedcomRecord> CrossReferencedRecords => _crossReferencedRecords;

        public IReadOnlyDictionary<GedcomPointer, GedcomIndividualRecord> IndividualRecords => _individualRecords;

        private void ProcessRecord(GedcomRecord record)
        {
            _records.Add(record);
            if (record.CrossReferenceId.HasValue)
            {
                _crossReferencedRecords.Add(record.CrossReferenceId.Value, record);
                if (record.Tag == GedcomIndividualRecord.Tag)
                    _individualRecords.Add(record.CrossReferenceId.Value, new GedcomIndividualRecord(record));
            }
            
        }
    }
}