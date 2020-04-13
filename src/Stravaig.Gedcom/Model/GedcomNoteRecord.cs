using System;
using System.Linq;
using System.Text;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomNoteRecord
    {
        public static readonly GedcomTag NoteTag = "NOTE".AsGedcomTag();
        public static readonly GedcomTag ContinuedTag = "CONT".AsGedcomTag();
        public static readonly GedcomTag ConcatenatedTag = "CONC".AsGedcomTag();
        private static readonly GedcomTag[] TextTags = new[] {ContinuedTag, ConcatenatedTag};

        private readonly GedcomRecord _record;
        private readonly GedcomDatabase _database;
        private readonly Lazy<string> _noteText;

        public GedcomNoteRecord(GedcomRecord record, GedcomDatabase database)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            _database = database ?? throw new ArgumentNullException(nameof(database));

            if (record.Tag != NoteTag)
                throw new ArgumentException("Expected a \"NOTE\" link record.", nameof(record));

            _noteText = new Lazy<string>(() =>
            {
                if (_record.Value.IsGedcomPointer())
                    return GetReferencedNote();
                
                bool isFirst = true;
                StringBuilder sb = new StringBuilder();
                if (record.Value.HasContent())
                {
                    sb.Append(_record.Value);
                    isFirst = false;
                }
                var textEntries = _record.Children.Where(r => TextTags.Contains(r.Tag));
                foreach (var textEntry in textEntries)
                {
                    if (isFirst)
                        isFirst = false;
                    else if (textEntry.Tag == ContinuedTag)
                        sb.AppendLine();
                    sb.Append(textEntry.Value);
                }

                return sb.ToString();
            });
        }

        private string GetReferencedNote()
        {
            var pointer = _record.Value.AsGedcomPointer();
            var note = _database.NoteRecords[pointer];
            return note.Text;
        }

        public string Text => _noteText.Value;
    }
}