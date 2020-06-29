using Stravaig.FamilyTree.Common.Footnotes;

namespace Stravaig.FamilyTree.Blazor.Services
{
    public class WebFootnoteOrganiser: FootnoteOrganiser, IWebFootnoteOrganiser
    {
        public Footnote[] Footnotes => _footnotes.ToArray();
    }
}