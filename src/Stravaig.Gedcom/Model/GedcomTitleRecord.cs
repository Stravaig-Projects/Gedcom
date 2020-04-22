using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomTitleRecord : MultiLineTextRecord
    {
        public static readonly GedcomTag TitleTag = "TITL".AsGedcomTag();
        public GedcomTitleRecord(GedcomRecord record, GedcomDatabase database)
            :base(record, database)
        {
            if (record.Tag != TitleTag)
                throw new ArgumentException($"Expected a \"TITL\" record, but got a \"{record.Tag}\" instead.");
        }
    }
}