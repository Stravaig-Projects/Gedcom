using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public class GedcomIndividualRecord
    {
        public static readonly GedcomTag Tag = "INDI".AsGedcomTag();
        public static readonly GedcomTag SexTag = "SEX".AsGedcomTag();
        
        private readonly GedcomRecord _record;
        private readonly Lazy<GedcomNameRecord[]> _lazyNames;

        public GedcomIndividualRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            if (record.Tag != Tag)
                throw new ArgumentException($"Must be an \"INDIVIDUAL_RECORD\" ({Tag}) record, but was {record.Tag}.");
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException("An \"INDIVIDUAL_RECORD\" must have a CrossReferenceId.");
            
            _lazyNames = new Lazy<GedcomNameRecord[]>(
                () => _record.Children
                    .Where(r => r.Tag == GedcomNameRecord.NameTag)
                    .Select(r => new GedcomNameRecord(r))
                    .ToArray());
        }

        // ReSharper disable once PossibleInvalidOperationException
        // Checked in the ctor.
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public string Name => (Names.FirstOrDefault(n => n.Type == string.Empty)
                              ?? Names.FirstOrDefault())?.Name;

        public GedcomNameRecord[] Names => _lazyNames.Value;
        
        public GedcomSex Sex => _record.Children.FirstOrDefault(r => r.Tag == SexTag)?.Value.AsGedcomSex() ?? GedcomSex.NotKnown;
        
        
    }
}