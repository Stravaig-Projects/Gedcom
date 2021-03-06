using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

// EVENT_DETAIL:=
//   n TYPE <EVENT_OR_FACT_CLASSIFICATION> {0:1} p.49
//   n DATE <DATE_VALUE> {0:1} p.47, 46
//   n <<PLACE_STRUCTURE>> {0:1} p.38
//   n <<ADDRESS_STRUCTURE>> {0:1} p.31
//   n AGNC <RESPONSIBLE_AGENCY> {0:1} p.60
//   n RELI <RELIGIOUS_AFFILIATION> {0:1} p.60
//   n CAUS <CAUSE_OF_EVENT> {0:1} p.43
//   n RESN <RESTRICTION_NOTICE> {0:1} p.60
//   n <<NOTE_STRUCTURE>> {0:M} p.37
//   n <<SOURCE_CITATION>> {0:M} p.39
//   n <<MULTIMEDIA_LINK>> {0:M} p.37, 26

namespace Stravaig.Gedcom.Model
{
    public abstract class EventRecord : Record
    {
        public static readonly GedcomTag TypeTag = "TYPE".AsGedcomTag();
        
        private readonly Lazy<GedcomDateRecord> _lazyDate;
        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;
        private readonly Lazy<GedcomSourceRecord[]> _lazySources;
        private readonly Lazy<GedcomAddressRecord> _lazyAddress;
        private readonly Lazy<GedcomPlaceRecord> _lazyPlace;
        protected EventRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            _lazyDate = new Lazy<GedcomDateRecord>(GetDateRecord);
            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazySources = new Lazy<GedcomSourceRecord[]>(GetSourceRecords);
            _lazyAddress = new Lazy<GedcomAddressRecord>(GetAddressRecord);
            _lazyPlace = new Lazy<GedcomPlaceRecord>(GetPlaceRecord);
        }

        private GedcomPlaceRecord GetPlaceRecord()
        {
            return MapChild(GedcomPlaceRecord.PlaceTag, GedcomPlaceRecord.Factory);
        }

        private GedcomAddressRecord GetAddressRecord()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == GedcomAddressRecord.AddressTag);
            if (record != null)
                return new GedcomAddressRecord(record, _database);

            return null;
        }

        public GedcomTag Tag => _record.Tag;
        public string RawValue => _record.Value;

        public GedcomDateRecord Date => _lazyDate.Value;

        public string Type => _record.Children
            .FirstOrDefault(r => r.Tag == TypeTag)?.Value;

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;
        public GedcomSourceRecord[] Sources => _lazySources.Value;
        public GedcomAddressRecord Address => _lazyAddress.Value;
        public GedcomPlaceRecord Place => _lazyPlace.Value;
    }
}