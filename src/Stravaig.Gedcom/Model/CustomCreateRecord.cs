using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class CustomCreateRecord : Record
    {
        public static readonly GedcomTag CreateTag = "_CRE".AsGedcomTag();
        private readonly GedcomDateRecord _dateTimeRecord;

        public CustomCreateRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            var dateTimeRecord = record.Children.FirstOrDefault(r => r.Tag == GedcomDateRecord.DateTag);
            _dateTimeRecord = new GedcomDateRecord(dateTimeRecord, database);
        }

        public GedcomDateRecord DateRecord => _dateTimeRecord;
    }
}