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
            public CalendarEscape Calendar { get; set; }
        
            public int? Day1 { get; set; }
            public int? Month1 { get; set; }
            public int? Year1 { get; set; }

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
                    $"\"{RawStringValue}\": Type={Type}, Calendar={Calendar}, Day1={Day1}, Month1={Month1}, Year1={Year1}, Day2={Day2}, Month2={Month2}, Year2={Year2}";
            }

            public TestCaseData Clone()
            {
                return new TestCaseData(RawStringValue)
                {
                    Type = Type,
                    Calendar = Calendar,
                    Day1 = Day1,
                    Month1 = Month1,
                    Year1 = Year1,
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
            _parser.Calendar.ShouldBe(data.Calendar);
            _parser.Day1.ShouldBe(data.Day1);
            _parser.Month1.ShouldBe(data.Month1);
            _parser.Year1.ShouldBe(data.Year1);
            _parser.Day2.ShouldBe(data.Day2);
            _parser.Month2.ShouldBe(data.Month2);
            _parser.Year2.ShouldBe(data.Year2);
        }

        public static IEnumerable<TestCaseData> HappyPathTestData()
        {
            var date3Apr2020 = new TestCaseData("@#DGREGORIAN@ 03 APR 2020")
            {
                Type = DateType.Date,
                Calendar = CalendarEscape.Gregorian,
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
                Calendar = CalendarEscape.Gregorian,
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
                Calendar = CalendarEscape.Gregorian,
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
                Calendar = CalendarEscape.Gregorian,
                Day1 = 5,
                Month1 = 9,
                Year1 = 1946,
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
        }
    }
}