using System;
using Stravaig.Gedcom.Constants;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model.Parsers;

namespace Stravaig.Gedcom.Model
{
    public class GedcomDateRecord : IComparable, IComparable<GedcomDateRecord>
    {
        private readonly GedcomRecord _record;
        public static readonly GedcomTag DateTag = "DATE".AsGedcomTag();
        
        private readonly Lazy<DateParser> _parser;
        
        public GedcomDateRecord(GedcomRecord record)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
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

        public bool IsSuccessful => string.IsNullOrWhiteSpace(_parser.Value.Error);
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

            if (Year1 == null || other.Year1 == null)
                return Order.ThisFollowsOther;

            if (this.Year1 < other.Year1)
                return Order.ThisPrecedesOther;
            if (this.Year1 > other.Year1)
                return Order.ThisFollowsOther;

            int thisMonth = this.Month1 ?? 6;
            int otherMonth = other.Month1 ?? 6;
            if (thisMonth < otherMonth)
                return Order.ThisPrecedesOther;
            if (thisMonth > otherMonth)
                return Order.ThisFollowsOther;

            int thisDay = this.Day1 ?? 15;
            int otherDay = this.Day1 ?? 15;
            if (thisDay < otherDay)
                return Order.ThisPrecedesOther;
            if (thisDay > otherDay)
                return Order.ThisFollowsOther;
            
            return Order.ThisOccursInTheSamePositionAsOther;
        }
    }
}