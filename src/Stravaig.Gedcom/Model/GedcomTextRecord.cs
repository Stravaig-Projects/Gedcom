using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomTextRecord : MultiLineTextRecord
    {
        public static readonly GedcomTag TextTag = "TEXT".AsGedcomTag();
        public GedcomTextRecord(GedcomRecord record, GedcomDatabase database)
            :base(record, database)
        {
            if (record.Tag != TextTag)
                throw new ArgumentException($"Expected a \"TEXT\" record, but got a \"{record.Tag}\" instead.");
        }
    }
}