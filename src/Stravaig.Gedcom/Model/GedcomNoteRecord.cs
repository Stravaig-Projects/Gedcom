using System;
using Stravaig.Gedcom.Extensions;

// NOTE_STRUCTURE:=
// [
//   n NOTE @<XREF:NOTE>@ {1:1} p.27
//   |
//   n NOTE [<SUBMITTER_TEXT> | <NULL>] {1:1} p.63
//     +1 [CONC|CONT] <SUBMITTER_TEXT> {0:M}
// ]
//
// Note: There are special considerations required when using the CONC tag. The
// usage is to provide a note string that can be concatenated together so that
// the display program can do its own word wrapping according to its display
// window size. The requirement for usage is to either break the text line in
// the middle of a word, or if at the end of a word, to add a space to the
// first of the next CONC line. Otherwise most operating systems will strip off
// the trailing space and the space is lost in the reconstitution of the note. 

namespace Stravaig.Gedcom.Model
{
    public class GedcomNoteRecord : MultiLineTextRecord
    {
        public static readonly GedcomTag NoteTag = "NOTE".AsGedcomTag();

        public GedcomNoteRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != NoteTag)
                throw new ArgumentException("Expected a \"NOTE\" record.", nameof(record));
        }

        public GedcomPointer? CrossReferenceId => _record.CrossReferenceId;
    }
}