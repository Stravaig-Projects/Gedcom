using System;
using System.Globalization;
using System.Linq;

// From THE GEDCOM STANDARD-release-5.5.1.pdf
// pages 45-48, 52, 65
//
// DATE_VALUE:=
//   [ <DATE> |
//     <DATE_PERIOD> |
//     <DATE_RANGE> |
//     <DATE_APPROXIMATED> |
//     INT <DATE> (<DATE_PHRASE>) |
//     (<DATE_PHRASE>) ]
//
// The DATE_VALUE represents the date of an activity, attribute, or event where:
// INT = Interpreted from knowledge about the associated date phrase included in parentheses. 
//
// An acceptable alternative to the date phrase choice is to use one of the other choices such as
// <DATE_APPROXIMATED> choice as the DATE line value and then include the date phrase value as a
// NOTE value subordinate to the DATE line tag.
//
// The date value can take on the date form of just a date, an approximated date, between a date
// and another date, and from one date to another date.  The preferred form of showing date
// imprecision, is to show, for example, MAY 1890 rather than ABT 12 MAY 1890.  This is because
// limits have not been assigned to the precision of the prefixes such as ABT or EST.  
//
// DATE :=
//   [ <DATE_CALENDAR_ESCAPE> | <NULL> ] <DATE_CALENDAR>
//
// DATE_CALENDAR_ESCAPE :=
//   [ @#DHEBREW@ | @#DROMAN@ | @#DFRENCH R@ | @#DGREGORIAN@ |  @#DJULIAN@ | @#DUNKNOWN@ ]
//
// The date escape determines the date interpretation by signifying which <DATE_CALENDAR> to use.
// The default calendar is the Gregorian calendar.
//
// DATE_CALENDAR :=
//   [ <DATE_GREG> | <DATE_JULN> | <DATE_HEBR> | <DATE_FREN> |  <DATE_FUTURE> ] 
//
// DATE_GREG:=
//   [ <YEAR_GREG>[B.C.] | <MONTH> <YEAR_GREG> |  <DAY> <MONTH> <YEAR_GREG> ]
//
// YEAR_GREG:=
//   [ <NUMBER> | <NUMBER>/<DIGIT><DIGIT> ]
//
// DATE_JULN:=
//   [ <YEAR>[B.C.] | <MONTH> <YEAR> | <DAY> <MONTH> <YEAR> ]
//
// The slash "/" <DIGIT><DIGIT> a year modifier which shows the possible date alternatives for
// pre-1752 date brought about by a changing the beginning of the year from MAR to JAN in the
// English calendar change of 1752, for example, 15 APR 1699/00.
// A (B.C.) appended to the <YEAR> indicates a date before the birth of Christ. 
//
// MONTH:=
//   [ JAN | FEB | MAR | APR | MAY | JUN |  JUL | AUG | SEP | OCT | NOV | DEC ]
// Where:
//   JAN = January
//   FEB = February
//   MAR = March
//   APR = April
//   MAY = May
//   JUN = June
//   JUL = July
//   AUG = August
//   SEP = September
//   OCT = October
//   NOV = November
//   DEC = December
//
// DAY:=
//   dd
//   Day of the month, where dd is a numeric digit whose value is within the valid range of the
//   days for the associated calendar month.
//
// DATE_PERIOD:=
//   [ FROM <DATE> |
//     TO <DATE> |
//     FROM <DATE> TO <DATE> ]
//
// Where:
//   FROM = Indicates the beginning of a happening or state.
//   TO = Indicates the ending of a happening or state.
//
// Examples:
//   FROM 1904 to 1915
//     = The state of some attribute existed from 1904 to 1915 inclusive.
//   FROM 1904
//     = The state of the attribute began in 1904 but the end date is unknown.
//   TO 1915
//     = The state ended in 1915 but the begin date is unknown.
//
// DATE_RANGE:=
//   [ BEF <DATE> |
//     AFT <DATE> |
//     BET <DATE> AND <DATE> ]
//
// Where:
//   AFT = Event happened after the given date.
//   BEF = Event happened before the given date.
//   BET = Event happened some time between date 1 AND date 2. For example, bet 1904 and 1915
//         indicates that the event state (perhaps a single day) existed somewhere between 1904
//         and 1915 inclusive.
//
// The date range differs from the date period in that the date range is an estimate that an event
// happened on a single date somewhere in the date range specified.
//
// DATE_APPROXIMATED:=
//   [ ABT <DATE> |
//     CAL <DATE> |
//     EST <DATE> ]
//
// Where:
//   ABT = About, meaning the date is not exact.
//   CAL = Calculated mathematically, for example, from an event date and age.
//   EST = Estimated based on an algorithm using some other event date.
//
// DATE_PHRASE:=
//   <TEXT>
// Any statement offered as a date when the year is not recognizable to a date parser, but which
// gives information about when an event occurred.

// ****************************************************************************


namespace Stravaig.Gedcom.Model.Parsers
{
    public class DateParser
    {
        private const string SymbolAbout = "ABT";
        private const string SymbolAfter = "AFT";
        private const string SymbolAnd = "AND";
        private const string SymbolBefore = "BEF";
        private const string SymbolBetween = "BET";
        private const string SymbolCalculated = "CAL";
        private const string SymbolEstimated = "EST";
        private const string SymbolFrom = "FROM";
        private const string SymbolInterpreted = "INT";
        private const string SymbolTo = "TO";

        private static readonly string[] MonthNames = new[]
        {
            // THE GEDCOM STANDARD-release-5.5.1.pdf
            // P52/53
            "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
        };

        private enum State
        {
            AtStart,
            FirstDate,
            SecondDate,
            NoMoreTokens
        }
        private string _currentToken;
        private int? _currentTokenAsInt;
        private int _position = -1;
        private State _state = State.AtStart;
        
        private string[] _tokens;

        public bool Parse(string rawDateValue)
        {
            if (string.IsNullOrWhiteSpace(rawDateValue))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(rawDateValue));
            _tokens = rawDateValue.Split();
            ResetParser();
            try
            {
                ParseDateValue();
                MoveNext();
                
            }
            catch (InvalidOperationException ioex)
            {
                string message = ioex.Message + Environment.NewLine;
                string upToErrorPoint = string.Join(" ", _tokens.Take(_position));
                string fromErrorPoint = string.Join(" ", _tokens.Skip(_position));
                message += $"  at character position {upToErrorPoint.Length}.{Environment.NewLine}"
                           + $"{upToErrorPoint} {fromErrorPoint}{Environment.NewLine}"
                           + new string(' ', upToErrorPoint.Length) + "^";
                Error = message;
                return false;
            }

            return true;
        }
        
        public string Error { get; private set; }
        public DateType Type { get; private set; }
        public CalendarEscape Calendar1 { get; private set; }
        public int? Day1 { get; private set; }
        public int? Month1 { get; private set; }
        public int? Year1 { get; private set; }

        public CalendarEscape Calendar2 { get; private set; }
        public int? Day2 { get; private set; }
        public int? Month2 { get; private set; }
        public int? Year2 { get; private set; }

        private void ResetParser()
        {
            _position = -1;
            _state = State.AtStart;
            Error = null;
            Type = DateType.Unknown;
            Calendar1 = CalendarEscape.Gregorian;
            Day1 = null;
            Month1 = null;
            Year1 = null;
            Calendar2 = CalendarEscape.Gregorian;
            Day2 = null;
            Month2 = null;
            Year2 = null;
            MoveNext();
        }
        
        private void MoveNext()
        {
            _position++;
            if (_position >= _tokens.Length)
            {
                _currentToken = null;
                _state = State.NoMoreTokens;
            }
            else
                _currentToken = _tokens[_position];
        }

        private void ParseDateValue()
        {
            if (_state == State.NoMoreTokens)
                return;

            _state = State.FirstDate;

            if (_currentToken.StartsWith("@#"))
            {
                Type = DateType.Date;                
                ParseDate();
                return;
            }

            if (IsCurrentTokenOneOf(SymbolFrom, SymbolTo))
            {
                ParseDatePeriod();
                return;
            }

            if (IsCurrentTokenOneOf(SymbolBefore, SymbolAfter, SymbolBetween))
            {
                ParseDateRange();
                return;
            }

            if (IsCurrentTokenOneOf(SymbolAbout, SymbolCalculated, SymbolEstimated))
            {
                ParseDateApproximated();
                return;
            }

            if (IsCurrentToken(SymbolInterpreted))
            {
                ParseDateInterpreted();
                return;
            }

            if (_currentToken.StartsWith("("))
            {
                ParseDatePhrase();
                return;
            }

            Type = DateType.Date;
            ParseDateGregorianOrJulian();
        }

        private bool IsCurrentToken(string symbol)
        {

            return _currentToken != null &&
                   _currentToken.Equals(symbol, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsCurrentTokenOneOf(params string[] symbols)
        {
            return _currentToken != null &&
                   symbols.Any(s => _currentToken.Equals(s, StringComparison.InvariantCultureIgnoreCase));
        }

        private void ParseDate()
        {
            if (_currentToken.StartsWith("@#"))
            {
                ParseCalendarEscape();
            }

            ParseDateGregorianOrJulian();
        }

        private void ParseDatePhrase()
        {
            throw new NotImplementedException();
        }

        private void ParseDateInterpreted()
        {
            throw new NotImplementedException();
        }

        private void ParseDateApproximated()
        {
            if (IsCurrentToken(SymbolAbout))
                Type = DateType.ApproximatedAbout;
            else if (IsCurrentToken(SymbolCalculated))
                Type = DateType.ApproximatedCalculated;
            else
                Type = DateType.ApproximatedEstimated;

            MoveNext();
            ParseDate();
        }

        private void ParseDateRange()
        {
            Type = DateType.Range;
            if (IsCurrentToken(SymbolBefore))
            {
                _state = State.SecondDate;
                MoveNext();
                ParseDate();
                MoveNext();
            }
            else if (IsCurrentToken(SymbolAfter))
            {
                _state = State.FirstDate;
                MoveNext();
                ParseDate();
                MoveNext();
            }
            else if (IsCurrentToken(SymbolBetween))
            {
                _state = State.FirstDate;
                MoveNext();
                ParseDate();
                if (IsCurrentToken(SymbolAnd))
                {
                    _state = State.SecondDate;
                    MoveNext();
                    ParseDate();
                }
                else
                {
                    throw new InvalidOperationException("Date range expected \"AND\" after first date, in the pattern \"BET <start-date> AND <end-date>\"");
                }
            }
        }

        private void ParseDatePeriod()
        {
            Type = DateType.Period;
            if (IsCurrentToken(SymbolFrom))
            {
                _state = State.FirstDate;
                MoveNext();
                ParseDate();
            }

            if (IsCurrentToken(SymbolTo))
            {
                _state = State.SecondDate;
                MoveNext();
                ParseDate();
            }
        }

        private void ParseCalendarEscape()
        {
            Action<CalendarEscape> setCalendar =
                _state == State.FirstDate
                    ? new Action<CalendarEscape>(c => Calendar1 = c)
                    : new Action<CalendarEscape>(c => Calendar2 = c);
            switch (_currentToken)
            {
                case "@#DHEBREW@":
                    setCalendar(CalendarEscape.Hebrew);
                    break;
                case "@#DROMAN@":
                    setCalendar(CalendarEscape.Roman);
                    break;
                case "@#DFRENCH":
                    MoveNext();
                    ParseCalendarEscape();
                    return;
                case "@#DFRENCHR@":
                case "R@":
                    setCalendar(CalendarEscape.French);
                    break;
                case "@#DGREGORIAN@":
                    setCalendar(CalendarEscape.Gregorian);
                    break;
                case "@#DJULIAN@":
                    setCalendar(CalendarEscape.Julian);
                    break;
                case "@#DUNKNOWN@":
                    setCalendar(CalendarEscape.Unknown);
                    break;
                default:
                    throw new InvalidOperationException($"Expected a Calendar Escape, but got \"{_currentToken}\".");
            }
            MoveNext();
        }

        private void ParseDateGregorianOrJulian()
        {
            if (CurrentTokenLooksLikeDay())
            {
                if (_state == State.FirstDate)
                    Day1 = _currentTokenAsInt;
                else
                    Day2 = _currentTokenAsInt;
                MoveNext();
            }

            if (CurrentTokenLooksLikeMonth())
            {
                if (_state == State.FirstDate)
                    Month1 = _currentTokenAsInt;
                else
                    Month2 = _currentTokenAsInt;
                MoveNext();
            }
                
            if (CurrentTokenLooksLikeYear())
            {
                if (_state == State.FirstDate)
                    Year1 = _currentTokenAsInt;
                else
                    Year2 = _currentTokenAsInt;
                MoveNext();
            }
        }

        private bool CurrentTokenLooksLikeMonth()
        {
            _currentTokenAsInt = null;
            for (int i = 0; i < MonthNames.Length; i++)
            {
                if (IsCurrentToken(MonthNames[i]))
                {
                    _currentTokenAsInt = i + 1;
                    return true;
                }
            }

            return false;
        }


        private bool CurrentTokenLooksLikeYear()
        {
            _currentTokenAsInt = null;
            if (!int.TryParse(_currentToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return false;
            if (result > 31)
            {
                _currentTokenAsInt = result;                
                return true;
            }
            return false;
        }
        
        private bool CurrentTokenLooksLikeDay()
        {
            _currentTokenAsInt = null;
            if (!int.TryParse(_currentToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return false;
            if (result <= 31)
            {
                _currentTokenAsInt = result;                
                return true;
            }
            return false;
        }
    }
}