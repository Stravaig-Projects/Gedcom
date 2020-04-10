using System;
using System.Collections.Generic;
using System.Diagnostics;
using Stravaig.Gedcom.Constants;
using Stravaig.Gedcom.Settings;

namespace Stravaig.Gedcom.Model.Comparers
{
    public class DateComparer : IComparer<GedcomDateRecord>
    {
        private const double DaysInYear = 365.0;
        private const double DaysInLeapYear = 366.0;
        private const double YearMidpoint = DaysInYear / 2.0;
        private const double LeapYearMidpoint = DaysInLeapYear / 2.0;
        
        private static DateComparer Instance { get; } = new DateComparer();
        
        public static int CompareDate(GedcomDateRecord x, GedcomDateRecord y)
        {
            return Instance.Compare(x, y);
        }
        public int Compare(GedcomDateRecord x, GedcomDateRecord y)
        {
            // Treat incoherent dates as null
            if (x != null && !x.HasCoherentDate)
                x = null;
            if (y != null && !y.HasCoherentDate)
                y = null;
            
            // Special processing for nulls
            if (x == null)
            {
                return y == null 
                    ? Order.XEqualsY 
                    : Order.XIsLessThanY;
            }

            if (y == null)
            {
                return Order.XIsGreaterThanY;
            }
            
            // Processing for real dates
            DateTime orderingDateX = OrderingDate(x);
            DateTime orderingDateY = OrderingDate(y);
            return orderingDateX.CompareTo(orderingDateY);
        }
        
        private DateTime OrderingDate(GedcomDateRecord date)
        {
            if (date.Type == DateType.Period || date.Type == DateType.Range)
                return GetDateTimeForExtent(date);

            return GetDateTimeForSinglePointDate(date);
        }

        private static DateTime GetDateTimeForExtent(GedcomDateRecord date)
        {
            if (date.Year1.HasValue && date.Year2.HasValue)
            {
                DateTime start = ConvertValuesToDateTime(date.Year1, date.Month1, date.Day1, date.OrderingRule);
                DateTime end = ConvertValuesToDateTime(date.Year2, date.Month2, date.Day2, date.OrderingRule);
                switch (date.OrderingRule)
                {
                    case DateOrderingRule.BeginningOfExtent:
                        return start;
                    case DateOrderingRule.MiddleOfExtent:
                        TimeSpan duration = end - start;
                        TimeSpan halfDuration = TimeSpan.FromTicks(duration.Ticks / 2);
                        DateTime result = start + halfDuration;
                        return result;
                    case DateOrderingRule.EndOfExtent:
                        return end;
                }
            }

            if (date.Year1.HasValue)
                return ConvertValuesToDateTime(date.Year1, date.Month1, date.Day1, date.OrderingRule);
            if (date.Year2.HasValue)
                return ConvertValuesToDateTime(date.Year2, date.Month2, date.Day2, date.OrderingRule);
            return DateTime.MaxValue;
        }

        private static DateTime GetDateTimeForSinglePointDate(GedcomDateRecord date)
        {
            return ConvertValuesToDateTime(date.Year1, date.Month1, date.Day1, date.OrderingRule);
        }

        private static DateTime ConvertValuesToDateTime(int? maybeYear, int? maybeMonth,
            int? maybeDay, DateOrderingRule orderingRule)
        {
            if (!maybeYear.HasValue)
                return DateTime.MaxValue;

            int year = maybeYear.Value;

            if (!maybeMonth.HasValue)
            {
                switch (orderingRule)
                {
                    case DateOrderingRule.BeginningOfExtent:
                        return new DateTime(year, 1, 1);
                    case DateOrderingRule.MiddleOfExtent:
                        return new DateTime(year, 1, 1)
                            .AddDays(DateTime.IsLeapYear(year)
                                ? LeapYearMidpoint
                                : YearMidpoint);
                    case DateOrderingRule.EndOfExtent:
                        return new DateTime(year, 12, 31);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            int month = maybeMonth.Value;

            if (!maybeDay.HasValue)
            {
                switch (orderingRule)
                {
                    case DateOrderingRule.BeginningOfExtent:
                        return new DateTime(year, month, 1);
                    case DateOrderingRule.MiddleOfExtent:
                        return new DateTime(year, month, 1)
                            .AddDays(DateTime.DaysInMonth(year, month) / 2.0);
                    case DateOrderingRule.EndOfExtent:
                        return new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            int day = maybeDay.Value;
            return new DateTime(year, month, day);
        }
    }
}