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
            // THE GEDCOM STANDARD-release-5.5.1.pdf
            // P52/53
            "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
        };

        public GedcomDateRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            if (record.Tag != DateTag)
                throw new ArgumentException($"The record must be a DATE type.");
        }

        public string RawDateValue => _record.Value;

        public GedcomDateApproximationType ApproximationType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_record.Value))
                    return GedcomDateApproximationType.NoValue;

                string[] parts = _record.Value
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                switch (parts[0])
                {
                    case "ABT":
                        return GedcomDateApproximationType.About;
                    case "CAL":
                        return GedcomDateApproximationType.Calculated;
                    case "EST":
                        return GedcomDateApproximationType.Estimated;
                    default:
                        return GedcomDateApproximationType.Exact;
                }
            }
        }
        
        
        public int? Year
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_record.Value))
                    return null;

                string yearPart = _record.Value
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(p => p.Length == 4 && p.All(char.IsDigit));

                if (int.TryParse(yearPart, out int result))
                    return result;
                return null;
            }
        }

        public int? Day
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_record.Value))
                    return null;

                string dayPart = _record.Value
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .FirstOrDefault(p => p.Length == 2 && p.All(char.IsDigit));
                
                if (int.TryParse(dayPart, out int result))
                    return result;
                return null;
            }
        }
        
        public int? Month
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_record.Value))
                    return null;

                string monthPart = _record.Value
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(p => p.Length == 3 && MonthNames.Contains(p));

                return MonthNameAsNumber(monthPart);
            }
        }

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
                        month = MonthNameAsNumber(parts[1]);
                    }
                    else
                        return null;
                    
                }

                if (!int.TryParse(parts[2], out int year) || year < 1 || year > 9999)
                    return null;

                return new DateTime(year, month, day);
            }
        }

        private static int MonthNameAsNumber(string monthPart)
        {
            int month = 0;
            foreach (var monthName in MonthNames)
            {
                month++;
                if (monthName == monthPart)
                    break;
            }

            return month;
        }
    }
}