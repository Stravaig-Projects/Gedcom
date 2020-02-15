using System;
using System.Diagnostics;

namespace Stravaig.Gedcom
{
    [DebuggerDisplay("{Level} {CrossReferenceId} {Tag} {Value}")]
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