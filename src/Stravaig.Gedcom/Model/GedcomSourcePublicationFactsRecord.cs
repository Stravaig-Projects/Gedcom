using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomSourcePublicationFactsRecord : MultiLineTextRecord
    {
        public static readonly GedcomTag PublicationTag = "PUBL".AsGedcomTag();
        public GedcomSourcePublicationFactsRecord(GedcomRecord record, GedcomDatabase database)
            :base(record, database)
        {
            if (record.Tag != PublicationTag)
                throw new ArgumentException($"Expected a \"PUBL\" record, but got a \"{record.Tag}\" instead.");
        }
    }
}