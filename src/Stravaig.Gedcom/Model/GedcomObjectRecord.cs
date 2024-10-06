using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomObjectRecord : Record
    {
        public static readonly GedcomTag ObjectTag = "OBJE".AsGedcomTag();

        private readonly Lazy<GedcomFileRecord> _fileRecord;
        private readonly Lazy<GedcomTitleRecord> _titleRecord;
        private Lazy<GedcomLabelRecord[]> _labels;

        public GedcomObjectRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            _fileRecord = new Lazy<GedcomFileRecord>(GetFileRecord);
            _titleRecord = new Lazy<GedcomTitleRecord>(GetTitleRecord);
            _labels = new Lazy<GedcomLabelRecord[]>(GetLabels);
        }

        public bool HasLabel(string labelName)
            => Labels.Any(l => l.Title.Equals(labelName, StringComparison.CurrentCultureIgnoreCase));

        public bool IsFileType(string typeName)
            => FileType.Equals(typeName, StringComparison.OrdinalIgnoreCase);

        private GedcomFileRecord GetFileRecord()
        {
            var fileRecord = _record.Children.FirstOrDefault(r => r.Tag == GedcomFileRecord.FileTag);
            if (fileRecord == null)
                return null;

            return new GedcomFileRecord(fileRecord, _database);
        }

        private GedcomTitleRecord GetTitleRecord()
        {
            var titleRecord = _record.Children.FirstOrDefault(r => r.Tag == GedcomTitleRecord.TitleTag);
            if (titleRecord == null)
                return null;

            return new GedcomTitleRecord(titleRecord, _database);
        }

        private GedcomLabelRecord[] GetLabels()
        {
            var labelRefs = _record.Children.Where(r => r.Tag == GedcomLabelRecord.LabelTag);
            var labelRecords = labelRefs
                .Where(r => !string.IsNullOrWhiteSpace(r.Value))
                .Select(r => _database.LabelRecords[r.Value.AsGedcomPointer()])
                .ToArray();

            return labelRecords;
        }

        public string Title => _titleRecord.Value?.Text;

        public string FileName => _fileRecord.Value?.Name;

        public string FileType => _fileRecord.Value?.Form;

        public GedcomLabelRecord[] Labels => _labels.Value;
    }
}
