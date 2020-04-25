using System;
using System.Globalization;
using System.Text;
using Humanizer;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Services
{
    public interface IDateRenderer
    {
        string RenderAsProse(GedcomDateRecord dateRecord);
        string RenderAsShortDate(GedcomDateRecord dateRecord);
    }

    public class DateRenderer: IDateRenderer
    {
        public string RenderAsProse(GedcomDateRecord dateRecord)
        {
            StringBuilder sb = new StringBuilder();
            string initialPreposition = ProseInitialPreposition(dateRecord);
            if (initialPreposition != null)
                sb.Append(initialPreposition);

            string date1 = ProseDate(dateRecord.Year1, dateRecord.Month1, dateRecord.Day1);
            if (date1 != null)
            {
                AddSpace(sb);
                sb.Append(date1);
            }

            string middlePreposition = ProseMiddlePreposition(dateRecord);
            if (middlePreposition != null)
            {
                AddSpace(sb);
                sb.Append(middlePreposition);
            }

            string date2 = ProseDate(dateRecord.Year2, dateRecord.Month2, dateRecord.Day2);
            if (date2 != null)
            {
                AddSpace(sb);
                sb.Append(date2);
            }

            return sb.ToString();
        }

        public string RenderAsShortDate(GedcomDateRecord dateRecord)
        {
            if (dateRecord == null)
                return string.Empty;
            
            StringBuilder sb = new StringBuilder();
            string prep = ShortInitialPreposition(dateRecord);
            if (prep != null)
                sb.Append(prep);
            
            string date1 = ShortDate(dateRecord.Year1, dateRecord.Month1, dateRecord.Day1);
            if (date1 != null)
                sb.Append(date1);

            prep = ShortMiddlePreposition(dateRecord);
            if (prep != null)
                sb.Append(prep);
            
            string date2 = ShortDate(dateRecord.Year2, dateRecord.Month2, dateRecord.Day2);
            if (date2 != null)
                sb.Append(date2);
            
            return sb.ToString();
        }

        private string ShortDate(int? year, int? month, int? day)
        {
            string format = string.Empty;
            if (day.HasValue)
                format += "d/";
            if (month.HasValue)
                format += "MMM/";
            if (year.HasValue)
                format += "yyyy";
            if (format == string.Empty)
                return null;
            DateTime date = new DateTime(year ?? 2020, month ?? 1, day ?? 1);
            return date.ToString(format);
        }

        private static void AddSpace(StringBuilder sb)
        {
            if (sb.Length != 0)
                sb.Append(" ");
        }

        private string ProseMiddlePreposition(GedcomDateRecord dateRecord)
        {
            switch (dateRecord.Type)
            {
                case DateType.Period:
                    if (dateRecord.Year2.HasValue)
                        return "to";
                    return null;
                case DateType.Range:
                    if (dateRecord.Year1.HasValue)
                    {
                        if (dateRecord.Year2.HasValue)
                            return "and";
                        return null;
                    }

                    if (dateRecord.Year2.HasValue)
                        return "before";
                    return null;
            }

            return null;
        }

        private string ShortMiddlePreposition(GedcomDateRecord dateRecord)
        {
            switch (dateRecord.Type)
            {
                case DateType.Period:
                    if (dateRecord.Year2.HasValue)
                        return " to ";
                    return null;
                case DateType.Range:
                    if (dateRecord.Year1.HasValue)
                    {
                        if (dateRecord.Year2.HasValue)
                            return " and ";
                        return null;
                    }

                    if (dateRecord.Year2.HasValue)
                        return "before ";
                    return null;
            }

            return null;
        }

        private string ProseDate(int? year, int? month, int? day)
        {
            StringBuilder sb = new StringBuilder();
            if (day.HasValue)
                sb.Append($"{day.Value.Ordinalize()} of ");

            if (month.HasValue)
            {
                string monthName = new DateTime(1, month.Value, 1).ToString("MMMM", CultureInfo.InvariantCulture);
                sb.Append(monthName);
                sb.Append(" ");
            }

            if (year.HasValue)
                sb.Append(year.Value.ToString());

            string result = sb.ToString();
            if (result == string.Empty)
                return null;
            return result;
        }

        private string ProseInitialPreposition(GedcomDateRecord dateRecord)
        {
            switch (dateRecord.Type)
            {
                case DateType.ApproximatedAbout:
                    return "about";
                case DateType.ApproximatedCalculated:
                    return "calculated to";
                case DateType.ApproximatedEstimated:
                    return "estimated as";
                case DateType.Date:
                case DateType.Interpreted:
                    if (dateRecord.Day1.HasValue)
                        return "on";
                    return ("in");
                case DateType.Period:
                    if (dateRecord.Year1.HasValue)
                        return "from";
                    return null;
                case DateType.Range:
                    if (dateRecord.Year1.HasValue)
                    {
                        if (dateRecord.Year2.HasValue)
                            return "between";
                        return "after";
                    }

                    return null;
            }

            return null;
        }
        private string ShortInitialPreposition(GedcomDateRecord dateRecord)
        {
            switch (dateRecord.Type)
            {
                case DateType.ApproximatedAbout:
                    return "about ";
                case DateType.ApproximatedCalculated:
                    return "calc'd ";
                case DateType.ApproximatedEstimated:
                    return "est'd ";
                case DateType.Date:
                case DateType.Interpreted:
                    return null;
                case DateType.Period:
                    if (dateRecord.Year1.HasValue)
                        return "from ";
                    return null;
                case DateType.Range:
                    if (dateRecord.Year1.HasValue)
                    {
                        if (dateRecord.Year2.HasValue)
                            return "bet' ";
                        return "after ";
                    }

                    return null;
            }

            return null;
        }
    }
}