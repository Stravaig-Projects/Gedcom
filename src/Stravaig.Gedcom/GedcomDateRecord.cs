using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public class GedcomDateRecord
    {
        private readonly GedcomRecord _record;
        public static readonly GedcomTag DateTag = "DATE".AsGedcomTag();

        private static readonly string[] MonthNames = new[]
        {
            "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
        };

        public GedcomDateRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            if (record.Tag != DateTag)
                throw new ArgumentException($"The record must be a DATE type.");
        }

        public string RawDateValue => _record.Value;
        
        public DateTime? ExactDate 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_record.Value))
                    return null;
                string[] parts = _record.Value.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                    return null;

                if (!int.TryParse(parts[0], out int day) || day < 1 || day > 31)
                    return null;

                if (!int.TryParse(parts[1], out int month) || month < 1 || month > 12)
                {
                    if (MonthNames.Any(m => m == parts[1]))
                    {
                        month = 0;
                        foreach (var monthName in MonthNames)
                        {
                            month++;
                            if (monthName == parts[1])
                                break;
                        }
                    }
                    else
                        return null;
                    
                }

                if (!int.TryParse(parts[2], out int year) || year < 1 || year > 9999)
                    return null;

                return new DateTime(year, month, day);
            }
        }
    }
}