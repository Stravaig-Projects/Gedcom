using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomUserReferenceNumberTypeRecord : Record
    {
        public static readonly GedcomTag ReferenceTypeTag = "REFT".AsGedcomTag();
        public GedcomUserReferenceNumberTypeRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != ReferenceTypeTag)
                throw new ArgumentException($"Expected a \"{ReferenceTypeTag}\" record, but got a \"{record.Tag}\" instead.");
        }
        
        public string Type => _record.Value;
    }
    
    public class GedcomUserReferenceNumberRecord : Record
    {
        public static readonly GedcomTag ReferenceTag = "REFN".AsGedcomTag();
        private static readonly GedcomTag TypeTag = "TYPE".AsGedcomTag();

        private readonly Lazy<string> _lazyType;
        
        public GedcomUserReferenceNumberRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != ReferenceTag)
                throw new ArgumentException($"Expected a \"REFN\" record, but got a \"{record.Tag}\" instead.");
            
            _lazyType = new Lazy<string>(GetTypeValue);
        }

        private string GetTypeValue()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == TypeTag);
            return record?.Value;
        }

        public string Reference => _record.Value;
        public string Type => _lazyType.Value;
    }
}