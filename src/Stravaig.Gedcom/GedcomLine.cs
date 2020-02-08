using System;

namespace Stravaig.Gedcom
{
    public class GedcomLine
    {
        public GedcomLevel Level { get; }
        public GedcomPointer? CrossReferenceId { get; }
        public GedcomTag Tag { get; }
        public string Value { get; }

        public GedcomLine(GedcomLevel level, GedcomPointer? crossReferenceId, GedcomTag tag, string value)
        {
            Tag = tag;
            Level = level;
            CrossReferenceId = crossReferenceId;
            Value = value;
        }
    }
}