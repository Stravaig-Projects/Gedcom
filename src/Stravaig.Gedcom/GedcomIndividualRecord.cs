using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public class GedcomIndividualRecord
    {
        public static readonly GedcomTag Tag = "INDI".AsGedcomTag();
        public static readonly GedcomTag NameTag = "NAME".AsGedcomTag();
        public static readonly GedcomTag SexTag = "SEX".AsGedcomTag();
        
        private readonly GedcomRecord _record;

        public GedcomIndividualRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            if (record.Tag != Tag)
                throw new ArgumentException($"Must be an \"INDIVIDUAL_RECORD\" ({Tag}) record, but was {record.Tag}.");
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException("An \"INDIVIDUAL_RECORD\" must have a CrossReferenceId.");
        }

        // ReSharper disable once PossibleInvalidOperationException
        // Checked in the ctor.
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public string Name => _record.Children.Single(r => r.Tag == NameTag).Value;

        public GedcomSex Sex => _record.Children.FirstOrDefault(r => r.Tag == SexTag)?.Value.AsGedcomSex() ?? GedcomSex.NotKnown;
        
        
    }
}