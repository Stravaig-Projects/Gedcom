using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomChangeRecord : Record
    {
        public static readonly GedcomTag ChangeTag = "CHAN".AsGedcomTag();

        private readonly GedcomDateRecord _dateTimeRecord;

        public GedcomChangeRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            var dateTimeRecord = record.Children.FirstOrDefault(r => r.Tag == GedcomDateRecord.DateTag);
            _dateTimeRecord = new GedcomDateRecord(dateTimeRecord, database);
        }

        public GedcomDateRecord DateRecord => _dateTimeRecord;
    }
}
