using System;
using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Parsers;

namespace Stravaig.Gedcom.UnitTests.Model.Parsers
{
    [TestFixture]
    public class DateParserTests
    {
        public class TestCaseData
        {
            public TestCaseData(string rawStringValue)
            {
                RawStringValue = rawStringValue;
            }
            public string RawStringValue { get; private set; }
            public DateType Type { get; set; }
            public string DatePhrase { get; set; }
            public CalendarEscape Calendar1 { get; set; }
            public int? Day1 { get; set; }
            public int? Month1 { get; set; }
            public int? Year1 { get; set; }

            public CalendarEscape Calendar2 { get; set; }
            public int? Day2 { get; set; }
            public int? Month2 { get; set; }
            public int? Year2 { get; set; }

            public TestCaseData SetRaw(string newRawStringValue)
            {
                var result = Clone();
                result.RawStringValue = newRawStringValue;
                return result;
            }

            public TestCaseData SetDay1(int? day1)
            {
                var result = Clone();
                result.Day1 = day1;
                return result;
            }
            
            public TestCaseData SetDay2(int? day2)
            {
                var result = Clone();
                result.Day2 = day2;
                return result;
            }
            
            public TestCaseData SetMonth1(int? month1)
            {
                var result = Clone();
                result.Month1 = month1;
                return result;
            }

            public TestCaseData SetMonth2(int? month2)
            {
                var result = Clone();
                result.Month2 = month2;
                return result;
            }
            
            public override string ToString()
            {
                return
                    $"\"{RawStringValue}\": Type={Type}, Calendar1={Calendar1}, Day1={Day1}, Month1={Month1}, Year1={Year1}, Calendar2={Calendar2}, Day2={Day2}, Month2={Month2}, Year2={Year2}";
            }

            public TestCaseData Clone()
            {
                return new TestCaseData(RawStringValue)
                {
                    Type = Type,
                    Calendar1 = Calendar1,
                    Day1 = Day1,
                    Month1 = Month1,
                    Year1 = Year1,
                    Calendar2 = Calendar2,
                    Day2 = Day2,
                    Month2 = Month2,
                    Year2 = Year2,
                };
            }
        }
        private DateParser _parser;
        
        [SetUp]
        public void SetUp()
        {
            _parser = new DateParser();
        }
        
        [Test]
        public void Parse_Null_ThrowsException()
        {
            Should.Throw<ArgumentException>(() => _parser.Parse(null));
        }

        [Test]
        [TestCaseSource(nameof(HappyPathTestData))]
        public void Parse_HappyPaths(TestCaseData data)
        {
            _parser.Parse(data.RawStringValue).ShouldBeTrue(_parser.Error);
            _parser.Type.ShouldBe(data.Type);
            _parser.Calendar1.ShouldBe(data.Calendar1);
            _parser.Day1.ShouldBe(data.Day1);
            _parser.Month1.ShouldBe(data.Month1);
            _parser.Year1.ShouldBe(data.Year1);
            _parser.Calendar2.ShouldBe(data.Calendar2);
            _parser.Day2.ShouldBe(data.Day2);
            _parser.Month2.ShouldBe(data.Month2);
            _parser.Year2.ShouldBe(data.Year2);
        }

        public static IEnumerable<TestCaseData> HappyPathTestData()
        {
            var date3Apr2020 = new TestCaseData("@#DGREGORIAN@ 03 APR 2020")
            {
                Type = DateType.Date,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 3,
                Month1 = 4,
                Year1 = 2020
            };
            yield return date3Apr2020;
            yield return date3Apr2020.SetRaw("03 APR 2020");
            yield return date3Apr2020.SetRaw("3 APR 2020");
            yield return date3Apr2020.SetRaw("@#DGREGORIAN@ 3 APR 2020");
            var dateApr2020 = date3Apr2020.SetDay1(null).SetRaw("@#DGREGORIAN@ APR 2020");
            yield return dateApr2020;
            yield return dateApr2020.SetRaw("APR 2020");
            var date2020 = dateApr2020.SetMonth1(null).SetRaw("@#DGREGORIAN@ 2020");
            yield return date2020;
            yield return date2020.SetRaw("2020");

            var periodFrom13Apr2012 = new TestCaseData("FROM @#DGREGORIAN@ 13 APR 2012")
            {
                Type = DateType.Period,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 13,
                Month1 = 4,
                Year1 = 2012
            };
            yield return periodFrom13Apr2012;
            yield return periodFrom13Apr2012.SetRaw("FROM 13 APR 2012");
            var periodFromApr2012 = periodFrom13Apr2012.SetDay1(null).SetRaw("FROM APR 2012");
            yield return periodFromApr2012;
            yield return periodFromApr2012.SetRaw("FROM @#DGREGORIAN@ APR 2012");
            var periodFrom2012 = periodFromApr2012.SetMonth1(null).SetRaw("FROM 2012");
            yield return periodFrom2012;
            yield return periodFrom2012.SetRaw("FROM @#DGREGORIAN@ 2012");

            var periodTo25Feb2019 = new TestCaseData("TO @#DGREGORIAN@ 25 FEB 2019")
            {
                Type = DateType.Period,
                Calendar2 = CalendarEscape.Gregorian,
                Day2 = 25,
                Month2 = 2,
                Year2 = 2019
            };
            yield return periodTo25Feb2019;
            yield return periodTo25Feb2019.SetRaw("TO 25 FEB 2019");
            var periodToFeb2019 = periodTo25Feb2019.SetDay2(null).SetRaw("TO FEB 2019");
            yield return periodToFeb2019;
            yield return periodToFeb2019.SetRaw("TO @#DGREGORIAN@ FEB 2019");
            var periodTo2019 = periodToFeb2019.SetMonth2(null).SetRaw("TO 2019");
            yield return periodTo2019;
            yield return periodTo2019.SetRaw("TO @#DGREGORIAN@ 2019");
            
            var periodFrom5Sep1946To24Nov1991 = new TestCaseData("FROM @#DGREGORIAN@ 5 SEP 1946 TO @#DGREGORIAN@ 24 NOV 1991")
            {
                Type = DateType.Period,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 5,
                Month1 = 9,
                Year1 = 1946,
                Calendar2 = CalendarEscape.Gregorian,
                Day2 = 24,
                Month2 = 11,
                Year2 = 1991
            };
            yield return periodFrom5Sep1946To24Nov1991;
            yield return periodFrom5Sep1946To24Nov1991.SetRaw("FROM 5 SEP 1946 TO 24 NOV 1991");
            var periodFromSep1946ToNov1992 = periodFrom5Sep1946To24Nov1991.SetDay1(null).SetDay2(null).SetRaw("FROM @#DGREGORIAN@ SEP 1946 TO @#DGREGORIAN@ NOV 1991");
            yield return periodFromSep1946ToNov1992;
            yield return periodFromSep1946ToNov1992.SetRaw("FROM SEP 1946 TO NOV 1991");
            var periodFrom1946To1992 = periodFromSep1946ToNov1992.SetMonth1(null).SetMonth2(null).SetRaw("FROM @#DGREGORIAN@ 1946 TO @#DGREGORIAN@ 1991");
            yield return periodFrom1946To1992;
            yield return periodFrom1946To1992.SetRaw("FROM 1946 TO 1991");
            
            var periodFrom31Oct1739To23June1795 = new TestCaseData("FROM @#DJULIAN@ 31 OCT 1739 TO @#DGREGORIAN@ 23 JUN 1795")
            {
                Type = DateType.Period,
                Calendar1 = CalendarEscape.Julian,
                Day1 = 31,
                Month1 = 10,
                Year1 = 1739,
                Calendar2 = CalendarEscape.Gregorian,
                Day2 = 23,
                Month2 = 6,
                Year2 = 1795
            };
            yield return periodFrom31Oct1739To23June1795;
            
            var rangeBetween25Jan1759And21July1796 = new TestCaseData("BET @#DGREGORIAN@ 25 JAN 1759 AND @#DGREGORIAN@ 21 JUL 1796")
            {
                Type = DateType.Range,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 25,
                Month1 = 1,
                Year1 = 1759,
                Calendar2 = CalendarEscape.Gregorian,
                Day2 = 21,
                Month2 = 7,
                Year2 = 1796
            };
            yield return rangeBetween25Jan1759And21July1796;
            yield return rangeBetween25Jan1759And21July1796.SetRaw("BET @#DGREGORIAN@ 25 JAN 1759 AND @#DGREGORIAN@ 21 JUL 1796");
            yield return rangeBetween25Jan1759And21July1796.SetRaw("BET 25 JAN 1759 AND 21 JUL 1796");
            var rangeBetweenJan1759AndJuly1796 = rangeBetween25Jan1759And21July1796.SetDay1(null).SetDay2(null).SetRaw("BET @#DGREGORIAN@ JAN 1759 AND @#DGREGORIAN@ JUL 1796");
            yield return rangeBetweenJan1759AndJuly1796;
            yield return rangeBetweenJan1759AndJuly1796.SetRaw("BET JAN 1759 AND JUL 1796");
            var rangeBetween1759And1796 = rangeBetweenJan1759AndJuly1796.SetMonth1(null).SetMonth2(null).SetRaw("BET @#DGREGORIAN@ 1759 AND @#DGREGORIAN@ 1796");
            yield return rangeBetween1759And1796;
            yield return rangeBetween1759And1796.SetRaw("BET 1759 AND 1796");
            
            
            var rangeAfter25Jan1759 = new TestCaseData("AFT @#DGREGORIAN@ 25 JAN 1759")
            {
                Type = DateType.Range,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 25,
                Month1 = 1,
                Year1 = 1759,
            };
            yield return rangeAfter25Jan1759;
            yield return rangeAfter25Jan1759.SetRaw("AFT 25 JAN 1759");
            var rangeAfterJan1759 = rangeAfter25Jan1759.SetDay1(null).SetRaw("AFT @#DGREGORIAN@ JAN 1759");
            yield return rangeAfterJan1759;
            yield return rangeAfterJan1759.SetRaw("AFT JAN 1759");
            var rangeAfter1759 = rangeAfterJan1759.SetMonth1(null).SetRaw("AFT @#DGREGORIAN@ 1759");
            yield return rangeAfter1759;
            yield return rangeAfter1759.SetRaw("AFT 1759");

            var rangeBefore21July1796 = new TestCaseData("BEF @#DGREGORIAN@ 21 JUL 1796")
            {
                Type = DateType.Range,
                Calendar2 = CalendarEscape.Gregorian,
                Day2 = 21,
                Month2 = 7,
                Year2 = 1796
            };
            yield return rangeBefore21July1796;
            yield return rangeBefore21July1796.SetRaw("BEF 21 JUL 1796");
            var rangeBeforeJuly1796 = rangeBefore21July1796.SetDay2(null).SetRaw("BEF @#DGREGORIAN@ JUL 1796");
            yield return rangeBeforeJuly1796;
            yield return rangeBeforeJuly1796.SetRaw("BEF JUL 1796");
            var rangeBefore1796 = rangeBeforeJuly1796.SetMonth2(null).SetRaw("BEF @#DGREGORIAN@ 1796");
            yield return rangeBefore1796;
            yield return rangeBefore1796.SetRaw("BEF 1796");
            
            var rangeFrom31Oct1739To23June1795 = new TestCaseData("BET @#DJULIAN@ 31 OCT 1739 AND @#DGREGORIAN@ 23 JUN 1795")
            {
                Type = DateType.Range,
                Calendar1 = CalendarEscape.Julian,
                Day1 = 31,
                Month1 = 10,
                Year1 = 1739,
                Calendar2 = CalendarEscape.Gregorian,
                Day2 = 23,
                Month2 = 6,
                Year2 = 1795
            };
            yield return rangeFrom31Oct1739To23June1795;
            
            var about4June1955 = new TestCaseData("ABT @#DGREGORIAN@ 4 Jun 1955")
            {
                Type = DateType.ApproximatedAbout,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 4,
                Month1 = 6,
                Year1 = 1955,
            };
            yield return about4June1955;
            yield return about4June1955.SetRaw("ABT 4 JUN 1955");
            var aboutJune1955 = about4June1955.SetDay1(null).SetRaw("ABT @#DGREGORIAN@ Jun 1955");
            yield return aboutJune1955;
            yield return aboutJune1955.SetRaw("ABT JUN 1955");
            var about1955 = aboutJune1955.SetMonth1(null).SetRaw("ABT @#DGREGORIAN@ 1955");
            yield return about1955;
            yield return about1955.SetRaw("ABT 1955");
            
            var calculated15May1987 = new TestCaseData("CAL @#DGREGORIAN@ 15 May 1987")
            {
                Type = DateType.ApproximatedCalculated,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 15,
                Month1 = 5,
                Year1 = 1987,
            };
            yield return calculated15May1987;
            yield return calculated15May1987.SetRaw("CAL 15 MAY 1987");
            var calculatedMay1987 = calculated15May1987.SetDay1(null).SetRaw("CAL @#DGREGORIAN@ May 1987");
            yield return calculatedMay1987;
            yield return calculatedMay1987.SetRaw("CAL MAY 1987");
            var calculated1987 = calculatedMay1987.SetMonth1(null).SetRaw("CAL @#DGREGORIAN@ 1987");
            yield return calculated1987;
            yield return calculated1987.SetRaw("CAL 1987");
            
            var estimated25November1835 = new TestCaseData("EST @#DGREGORIAN@ 25 NOV 1835")
            {
                Type = DateType.ApproximatedEstimated,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 25,
                Month1 = 11,
                Year1 = 1835,
            };
            yield return estimated25November1835;
            yield return estimated25November1835.SetRaw("EST 25 NOV 1835");
            var estimatedNovember1835 = estimated25November1835.SetDay1(null).SetRaw("EST @#DGREGORIAN@ NOV 1835");
            yield return estimatedNovember1835;
            yield return estimatedNovember1835.SetRaw("EST NOV 1835");
            var estimated1835 = estimatedNovember1835.SetMonth1(null).SetRaw("EST @#DGREGORIAN@ 1835");
            yield return estimated1835;
            yield return estimated1835.SetRaw("EST 1835");

            var interpreted4Jul1776 = new TestCaseData("INT @#DGREGORIAN@ 4 JUL 1776 (July 4th '76)")
            {
                Type = DateType.Interpreted,
                Calendar1 = CalendarEscape.Gregorian,
                Day1 = 4,
                Month1 = 7,
                Year1 = 1776,
                DatePhrase = "July 4th 1776",
            };
            yield return interpreted4Jul1776;
            yield return interpreted4Jul1776.SetRaw("INT 4 JUL 1776 (July 4th '76)");
            
            var phrase = new TestCaseData("(Easter)")
            {
                Type = DateType.Phrase,
                Calendar1 = CalendarEscape.Gregorian,
                DatePhrase = "Easter"
            };
            yield return phrase;
        }
    }
}