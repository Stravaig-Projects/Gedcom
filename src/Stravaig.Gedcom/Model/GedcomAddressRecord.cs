using System;
using Stravaig.Gedcom.Extensions;

// ADDRESS_STRUCTURE:=
//   n ADDR <ADDRESS_LINE> {1:1} p.41
//     +1 CONT <ADDRESS_LINE> {0:3} p.41
//     +1 ADR1 <ADDRESS_LINE1> {0:1} p.41
//     +1 ADR2 <ADDRESS_LINE2> {0:1} p.41
//     +1 ADR3 <ADDRESS_LINE3> {0:1} p.41
//     +1 CITY <ADDRESS_CITY> {0:1} p.41
//     +1 STAE <ADDRESS_STATE> {0:1} p.42
//     +1 POST <ADDRESS_POSTAL_CODE> {0:1} p.41
//     +1 CTRY <ADDRESS_COUNTRY> {0:1} p.41
//   n PHON <PHONE_NUMBER> {0:3} p.57
//   n EMAIL <ADDRESS_EMAIL> {0:3} p.41
//   n FAX <ADDRESS_FAX> {0:3} p.41
//   n WWW <ADDRESS_WEB_PAGE> {0:3} p.42

namespace Stravaig.Gedcom.Model
{
    public class GedcomAddressRecord : MultiLineTextRecord
    {
        public static readonly GedcomTag AddressTag = "ADDR".AsGedcomTag();

        public GedcomAddressRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != AddressTag)
                throw new ArgumentException($"Expected an \"ADDR\" record, but got a \"{record.Tag}\" record.", nameof(record));
        }
    }
}