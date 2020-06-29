using Stravaig.FamilyTree.Common.Footnotes;

namespace Stravaig.FamilyTree.Blazor.Services
{
    public interface IWebFootnoteOrganiser : IFootnoteOrganiser
    {
        public FootnoteOrganiser.Footnote[] Footnotes { get; }
    }
}