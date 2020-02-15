using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Stravaig.Gedcom
{
    [DebuggerDisplay("{Level} {CrossReferenceId} {Tag} {Value} with {Children.Count} children")]

    public class GedcomRecord
    {
        private static readonly GedcomRecord[] EmptyArray = new GedcomRecord[0];
        private readonly GedcomLine _line;
        private readonly GedcomRecord _parent;
        private readonly List<GedcomRecord> _children = new List<GedcomRecord>();
        public GedcomRecord(GedcomLine line, GedcomRecord parent)
            :this(line)
        {
            _parent = parent;
            _parent.AttachChild(this);
        }

        public GedcomRecord(GedcomLine line)
        {
            _line = line;
            _parent = null;
        }

        public static GedcomRecord From(GedcomLine line, GedcomRecord parent)
        {
            return parent == null 
                ? new GedcomRecord(line) 
                : new GedcomRecord(line, parent);
        }
        
        public GedcomTag Tag => _line.Tag;
        public GedcomLevel Level => _line.Level;
        public GedcomPointer? CrossReferenceId => _line.CrossReferenceId;
        public string Value => _line.Value;
        public IReadOnlyList<GedcomRecord> Children => _children;
        public GedcomRecord Parent => _parent;
        public IReadOnlyList<GedcomRecord> SiblingsInclusive => _parent?.Children ?? EmptyArray;
        public IReadOnlyList<GedcomRecord> SiblingsExclusive => SiblingsInclusive.Where(s => s != this).ToArray();
        
        public void AttachChild(GedcomRecord child)
        {
            _children.Add(child);
        }
    }
}