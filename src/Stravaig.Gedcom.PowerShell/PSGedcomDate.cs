using System;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell
{
    // ReSharper disable once InconsistentNaming
    // This is the PowerShell standard naming.
    public class PSGedcomDate
    {
        private readonly GedcomDateRecord _date;

        public PSGedcomDate(GedcomDateRecord date)
        {
            _date = date;
        }

        public DateType Type => _date.Type;
        public DateTime? BeginningOfExtent => _date.BeginningOfExtent;
        public DateTime? EndOfExtent => _date.EndOfExtent;
        public bool HasCoherentDate => _date.HasCoherentDate;
        
        

        public override string ToString()
        {
            return _date.RawDateValue;
        }
    }
}