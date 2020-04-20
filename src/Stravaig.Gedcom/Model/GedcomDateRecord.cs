using System;
using System.Diagnostics;
using Stravaig.Gedcom.Constants;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model.Comparers;
using Stravaig.Gedcom.Model.Parsers;
using Stravaig.Gedcom.Settings;

namespace Stravaig.Gedcom.Model
{
    [DebuggerDisplay("{RawDateValue}")]
    public class GedcomDateRecord : IComparable, IComparable<GedcomDateRecord>
    {
        private readonly GedcomRecord _record;
        private readonly GedcomDatabase _database;
        public static readonly GedcomTag DateTag = "DATE".AsGedcomTag();
        
        private readonly Lazy<DateParser> _parser;
        
        public GedcomDateRecord(GedcomRecord record, GedcomDatabase database)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            if (record.Tag != DateTag)
                throw new ArgumentException($"The record must be a DATE type.");
            
            _parser = new Lazy<DateParser>( () =>
            {
                var result = new DateParser();
                result.Parse(this.RawDateValue);
                return result;
            });
        }

        public string RawDateValue => _record.Value;
        public DateOrderingRule OrderingRule => _database.Settings.DateOrderingRule;
        public bool IsSuccessful => string.IsNullOrWhiteSpace(_parser.Value.Error);
        public bool HasCoherentDate => IsSuccessful && Type != DateType.Phrase;
        public string Error => _parser.Value.Error;
        public DateType Type => _parser.Value.Type;
        public int? Year1 => _parser.Value.Year1;
        public int? Month1 => _parser.Value.Month1;
        public int? Day1 => _parser.Value.Day1;
        public CalendarType Calendar1 => _parser.Value.Calendar1;

        public int? Year2 => _parser.Value.Year2;
        public int? Month2 => _parser.Value.Month2;
        public int? Day2 => _parser.Value.Day2;
        public CalendarType Calendar2 => _parser.Value.Calendar2;

        public DateTime? ExactDate1 
        {
            get
            {
                var parser = _parser.Value;
                if (parser.Day1.HasValue && parser.Month1.HasValue && parser.Year1.HasValue)
                    return new DateTime(parser.Year1.Value, parser.Month1.Value, parser.Day1.Value);
                return null;
            }
        }
        
        public DateTime? ExactDate2 
        {
            get
            {
                var parser = _parser.Value;
                if (parser.Day2.HasValue && parser.Month2.HasValue && parser.Year2.HasValue)
                    return new DateTime(parser.Year2.Value, parser.Month2.Value, parser.Day2.Value);
                return null;
            }
        }

        public DateTime? EndOfExtent
        {
            get
            {
                if (HasCoherentDate)
                {
                    int year = 9999;
                    if (Year2.HasValue)
                        year = Year2.Value;
                    else if (Year1.HasValue)
                        year = Year1.Value;
                    int month = 12;
                    if (Month2.HasValue)
                        month = Month2.Value;
                    else if (Month1.HasValue)
                        month = Month1.Value;
                    int day = DateTime.DaysInMonth(year, month);
                    if (Day2.HasValue)
                        day = Day2.Value;
                    else if (Day1.HasValue)
                        day = Day1.Value;
                    return new DateTime(year, month, day);
                }

                return null;
            }
        }

        public DateTime? BeginningOfExtent
        {
            get
            {
                if (HasCoherentDate)
                {
                    int year = 1;
                    if (Year1.HasValue)
                        year = Year1.Value;
                    else if (Year2.HasValue)
                        year = Year2.Value;
                    int month = 1;
                    if (Month1.HasValue)
                        month = Month1.Value;
                    else if (Month2.HasValue)
                        month = Month2.Value;
                    int day = 1;
                    if (Day1.HasValue)
                        day = Day1.Value;
                    else if (Day2.HasValue)
                        day = Day2.Value;
                    return new DateTime(year, month, day);
                }

                return null;
            }
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return Order.ThisFollowsOther;
                case GedcomDateRecord other:
                    return CompareTo(other);
                default:
                    throw new ArgumentException($"Cannot compare {GetType().FullName} to {obj.GetType().FullName}",
                        nameof(obj));
            }
        }

        public int CompareTo(GedcomDateRecord other)
        {
            if (other == null)
                return Order.ThisFollowsOther;

            return DateComparer.CompareDate(this, other);
        }

        public bool IsBetween(GedcomDateRecord start, GedcomDateRecord end, bool inclusive = true)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            
            int startComparison = DateComparer.CompareDate(start, this);
            if (startComparison == Order.XIsLessThanY || 
                (inclusive && startComparison == Order.XEqualsY))
            {
                int endComparison = DateComparer.CompareDate(this, end);
                if (endComparison == Order.XIsLessThanY || (inclusive && endComparison == Order.XEqualsY))
                {
                    return true;
                }
            }

            return false;
        }
    }
}