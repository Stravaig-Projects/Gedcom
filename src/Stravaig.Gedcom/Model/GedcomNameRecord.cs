using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomNameRecord : Record
    {
        public static readonly GedcomTag NameTag = "NAME".AsGedcomTag();
        public static readonly GedcomTag TypeTag = "TYPE".AsGedcomTag();

        public GedcomNameRecord(GedcomRecord record, GedcomDatabase database) 
            : base(record, database)
        {
            if (_record.Tag != NameTag)
                throw new ArgumentException($"Expected a Name record but got \"{_record.Tag}\".");
        }

        public string Name => _record.Value;

        public string WholeName => Name.Replace("/", " ")
            .Replace("  ", " ");

        public string Surname => Name.Count(c => c == '/') != 2
            ? string.Empty
            : Name.Substring(Name.IndexOf('/')+1, Name.LastIndexOf('/')-1);

        public string Type => _record.Children
            .SingleOrDefault(r => r.Tag == TypeTag)
            ?.Value ?? string.Empty;
    }
}