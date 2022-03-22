using System;
using Stravaig.Gedcom.Extensions;

// PLACE_STRUCTURE:=
//   n PLAC <PLACE_NAME> {1:1} p.58
//     +1 FORM <PLACE_HIERARCHY> {0:1} p.58
//     +1 FONE <PLACE_PHONETIC_VARIATION> {0:M} p.59
//       +2 TYPE <PHONETIC_TYPE> {1:1} p.57
//     +1 ROMN <PLACE_ROMANIZED_VARIATION> {0:M} p.59
//       +2 TYPE <ROMANIZED_TYPE> {1:1} p.61
//     +1 MAP {0:1}
//       +2 LATI <PLACE_LATITUDE> {1:1} p.58
//       +2 LONG <PLACE_LONGITUDE> {1:1} p.58
//     +1 <<NOTE_STRUCTURE>> {0:M} p.37
//
namespace Stravaig.Gedcom.Model
{
    public class GedcomPlaceRecord : Record
    {
        public static readonly GedcomTag PlaceTag = "PLAC".AsGedcomTag();
        public static readonly GedcomTag FormTag = "FORM".AsGedcomTag();

        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;
        private readonly Lazy<GedcomMapRecord> _lazyMap;
        
        public GedcomPlaceRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != PlaceTag)
                throw new ArgumentException($"Expected an \"PLAC\" record, but got a \"{record.Tag}\" record.", nameof(record));
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazyMap = new Lazy<GedcomMapRecord>(GetMapRecord);
        }

        public static GedcomPlaceRecord Factory(GedcomRecord record, GedcomDatabase database)
        {
            return new GedcomPlaceRecord(record, database);
        }

        private GedcomMapRecord GetMapRecord()
        {
            return MapChild(GedcomMapRecord.MapTag, GedcomMapRecord.Factory);
        }

        public string Name => _record.Value;
        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
        public GedcomMapRecord MapRecord => _lazyMap.Value;

    }
}