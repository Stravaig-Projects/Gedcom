using System;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    // NOTE: This was removed from the GEDCOM standard in v5.4 but is still
    // used by some software
    public class GedcomLabelRecord : Record
    {
        public static readonly GedcomTag LabelTag = "LABL".AsGedcomTag();
        private static readonly GedcomTag TitleTag = "TITL".AsGedcomTag();
        private static readonly GedcomTag ColourTag = "COLR".AsGedcomTag();

        private readonly Lazy<string> _lazyTitle;
        
        public GedcomLabelRecord(GedcomRecord record, GedcomDatabase database) 
            : base(record, database)
        {
            if (record.Tag != LabelTag)
                throw new ArgumentException($"Must be a \"LABEL\" ({LabelTag}) record, but was {record.Tag}.");
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException("A \"LABEL\" must have a CrossReferenceId.");
            
            _lazyTitle = new Lazy<string>(GetTitle);
        }

        private string GetTitle()
        {
            return GetChild(TitleTag)?.Value;
        }

        public string Title => _lazyTitle.Value;
        public string Colour => GetChild(ColourTag)?.Value;
    }
}