using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Common.Footnotes
{
    public interface IFootnoteOrganiser
    {
        int AddFootnote(GedcomSourceRecord source);
        int AddFootnote(GedcomNoteRecord note);

        bool HasFootnote(GedcomSourceRecord source);

        bool HasFootnote(GedcomNoteRecord note);
    }
}
