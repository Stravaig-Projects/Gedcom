using System;
using System.Diagnostics;
using System.Text;

namespace Stravaig.Gedcom
{
    [DebuggerDisplay("{Level} {CrossReferenceId} {Tag} {Value}")]
    public class GedcomLine
    {
        public GedcomLevel Level { get; }
        public GedcomPointer? CrossReferenceId { get; }
        public GedcomTag Tag { get; }
        public string Value { get; }

        public GedcomLine(GedcomLevel level, GedcomTag tag)
            : this (level, null, tag, null) 
        {
        }

        public GedcomLine(GedcomLevel level, GedcomPointer crossReferenceId, GedcomTag tag)
            : this(level, crossReferenceId, tag, null)
        {
        }

        public GedcomLine(GedcomLevel level, GedcomTag tag, string value)
            : this(level, null, tag, value)
        {
        }
        
        public GedcomLine(GedcomLevel level, GedcomPointer? crossReferenceId, GedcomTag tag, string value)
        {
            Tag = tag;
            Level = level;
            CrossReferenceId = crossReferenceId;
            Value = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Level);
            sb.Append(" ");
            if (CrossReferenceId.HasValue)
            {
                sb.Append(CrossReferenceId);
                sb.Append(" ");
            }

            sb.Append(Tag);
            if (!string.IsNullOrWhiteSpace(Value))
            {
                sb.Append(" ");
                sb.Append(Value);
            }

            return sb.ToString();
        }
    }
}