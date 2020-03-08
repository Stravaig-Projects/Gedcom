using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public class GedcomNameRecord
    {
        public static readonly GedcomTag NameTag = "NAME".AsGedcomTag();
        public static readonly GedcomTag TypeTag = "TYPE".AsGedcomTag();

        private readonly GedcomRecord _record;
        public GedcomNameRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            if (_record.Tag != NameTag)
                throw new ArgumentException($"Expected a Name record but got \"{_record.Tag}\".");
        }

        public string Name => _record.Value;

        public string Type => _record.Children
            .SingleOrDefault(r => r.Tag == TypeTag)
            ?.Value ?? string.Empty;
    }
}