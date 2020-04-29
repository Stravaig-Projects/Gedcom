using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class GedcomDateRecordFileNameExtensions
    {
        public static string ForFileName(this GedcomDateRecord date)
        {
            if (date == null || !date.HasCoherentDate)
                return "x";

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> parts = GetFirstDateParts(date);
            sb.Append(string.Join('-', parts));
            
            if (HasTwoDateParts(date))
                sb.Append("~");

            parts = GetSecondDateParts(date);
            sb.Append(string.Join('-', parts));

            return sb.ToString();
        }

        private static bool HasTwoDateParts(GedcomDateRecord date)
        {
            return (date.Year1.HasValue || date.Month1.HasValue || date.Day1.HasValue) &&
                   (date.Year2.HasValue || date.Month2.HasValue || date.Day2.HasValue);
        }

        private static IEnumerable<string> GetFirstDateParts(GedcomDateRecord date)
        {
            if (date.Year1.HasValue)
                yield return date.Year1.Value.ToString(CultureInfo.InvariantCulture);

            if (date.Month1.HasValue)
                yield return date.Month1.Value.ToString(CultureInfo.InvariantCulture);

            if (date.Day1.HasValue)
                yield return date.Day1.Value.ToString(CultureInfo.InvariantCulture);
        }

        private static IEnumerable<string> GetSecondDateParts(GedcomDateRecord date)
        {
            if (date.Year2.HasValue)
                yield return date.Year2.Value.ToString(CultureInfo.InvariantCulture);

            if (date.Month2.HasValue)
                yield return date.Month2.Value.ToString(CultureInfo.InvariantCulture);

            if (date.Day2.HasValue)
                yield return date.Day2.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}