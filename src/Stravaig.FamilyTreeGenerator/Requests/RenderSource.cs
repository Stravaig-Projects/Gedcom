using Stravaig.FamilyTreeGenerator.Requests.Models;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderSource : Request
    {
        public SourceEntry SourceEntry { get; }

        public RenderSource(SourceEntry sourceEntry)
        {
            SourceEntry = sourceEntry;
        }
    }
}